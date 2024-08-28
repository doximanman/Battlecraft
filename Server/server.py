import asyncio
import json
from websockets.asyncio.server import serve

ip = 'localhost'
port = 3000

import router
async def sendToRouter(websocket):
    message = await websocket.recv()
    messageObject = json.loads(message)
    requestType = messageObject['type']
    del messageObject['type']
    if requestType == 'user':
        await router.user(messageObject)
    elif requestType == 'player':
        await router.player(messageObject)
    elif requestType == 'world':
        await router.world(messageObject)
    else:
        await router.default(messageObject)

async def main():
    async with serve(sendToRouter,ip,port):
        await asyncio.get_running_loop().create_future()
