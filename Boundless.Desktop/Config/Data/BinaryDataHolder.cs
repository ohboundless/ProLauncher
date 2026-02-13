using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;

namespace Boundless.Desktop.Config.Data;

public class BinaryDataHolder
{
    public virtual string FileName { get; }
    private string FilePath => Path.Combine(Configuration.DirectoryName, FileName);
    
    #region LOADING AND SAVING
    
    public bool Read()
    {
        if (!File.Exists(FilePath))
        {   
            Logger.Information($"Cannot load '{FilePath}' because it does not exist.");
            return false;
        }

        _reader = new BinaryReader(File.OpenRead(FilePath));
        
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType == typeof(IEnumerable))
            {
                var type = property.PropertyType.GetGenericArguments()[0];
                var methodDef = typeof(BinaryDataHolder).GetMethod(nameof(ReadList));
                var methodInst = methodDef!.MakeGenericMethod(type);
                
                var result = methodInst.Invoke(this, []);
                property.SetValue(this, result);
            }
            else
            {
                var type = property.PropertyType;
                var methodDef = typeof(BinaryDataHolder).GetMethod(nameof(Read));
                var methodInst = methodDef!.MakeGenericMethod(type);
                
                var result = methodInst.Invoke(this, []);
                property.SetValue(this, result);
            }
            
            Logger.Information($"Loaded '{property.Name}'...");
        }
        
        _reader.Close();
        // Technically could delete _reader instance but who cares...

        return true;
    }
    
    public void Save()
    {
        Logger.Information("Opening binary file...");
        _writer = new BinaryWriter(File.Exists(FilePath) ? File.OpenWrite(FilePath) : File.Create(FilePath));
        Logger.Information($"Opened '{FilePath}'");
        
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(this);

            if (value is IEnumerable enumerable)
            {
                var list = (List<object>)enumerable;
                WriteList(list);
            }
            else
            {
                Write(value);
            }
            
            Logger.Information($"Saved '{property.Name}'...");
        }
        
        _writer.Close();
    }

    public void Delete()
    {
        File.Delete(FilePath);
    }
    
    #endregion

    #region READING

    private BinaryReader _reader;
    
    private T Read<T>()
    {
        var buffer = new byte[Unsafe.SizeOf<T>()];
        var read = _reader.Read(buffer);

        if (buffer.Length != read)
            throw new DataException($"'{typeof(T).Name}' did not read the expected amount of bytes.");
        
        return Unsafe.ReadUnaligned<T>(ref buffer[0]);
    }

    private List<T> ReadList<T>()
    {
        var result = new List<T>();
        var length = Read<int>();

        for (int i = 0; i < length; i++)
            result.Add(Read<T>());

        return result;
    }

    #endregion
    
    #region WRITING

    private BinaryWriter _writer;
    
    private void Write<T>(T value)
    {
        var buffer = new byte[Unsafe.SizeOf<T>()];
        Unsafe.WriteUnaligned(ref buffer[0], value);
        
        _writer.Write(buffer);
    }

    private void WriteList<T>(List<T> values)
    {
        Write(values.Count);
        foreach (var value in values)
            Write(value);    
    }

    #endregion
}