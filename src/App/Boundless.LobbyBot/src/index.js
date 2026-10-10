import fnbr from 'fnbr'
import express from 'express'

import deviceAuth from './../device_auth.json' with { type: 'json' }

const { Enums } = fnbr;
const { PartyPrivacy } = Enums;

let client = new fnbr.Client({
    auth: {
        deviceAuth: deviceAuth
    },
    defaultStatus: 'Pro Swapper Lobby Bot',
    partyConfig: {
        joinability: 'INVITE_AND_FORMER',
        privacy: PartyPrivacy.PRIVATE
    }
});

client.on('ready', async () => {
    console.log(`Client is ready (${client.user.self.displayName})`)
})

client.on('party:invite', async (invitation) => {
    invitation.accept();
});

let outfit = null;
let backpack = null;

client.on('party:member:joined', async (member) => {
    if (outfit)
        setCosmetic('AthenaCharacter', outfit);

    if (backpack)
        setCosmetic('AthenaBackpack', backpack);

    client.party.me.setSittingOut(true);
});

const app = express();
const PORT = 7701;

app.get('/set', (req, res) => {
    try {
        const type = req.query['type'];
        const id = req.query['id'];
        
        setCosmetic(type, id);
        res.send(200);
    }
    catch (ex) {
        res.status(500).send(ex);
    }
});

app.listen(PORT, () => {
    console.log(`Server is running at http://localhost:${PORT}`);
})

function setCosmetic(type, id)
{
    switch (type) {
        case 'AthenaCharacter':
            outfit = id;
            client.party.me.setOutfit(id);
            break;
        case 'AthenaBackpack':
            backpack = id;
            client.party.me.setBackpack(id);
            break;
    }

    console.log(`Applied cosmetic: ${type}:${id}`);
}

await client.login();