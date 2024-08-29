import asyncio
import json
from websockets.asyncio.server import serve


# get ip and port from command line arguments
import sys
if len(sys.argv) < 3:
    print("IP and port must be provided")
    exit(1)

ip = sys.argv[1]
port= -1
try:
    port = int(sys.argv[2])
except ValueError:
    print("Port must be a number")
    exit(1)

if port <= 0 or port > 65535:
    print("Invalid port number")
    exit(1)


mongo_ip = 'localhost'
mongo_port = 27017

# optionally, provide mongodb ip and port
if len(sys.argv) >= 5:
    try_mongo_ip = sys.argv[3]
    try_mongo_port = -1
    try:
        try_mongo_port = int(sys.argv[4])
    except ValueError:
        print("Mongodb port must be a number")

    if try_mongo_port <= 0 or try_mongo_port > 65535:
        print("Invalid mongodb port")
        try_mongo_port = -1

    if try_mongo_port != -1:
        mongo_ip = try_mongo_ip
        mongo_port = try_mongo_port

print(f'Mongodb address: {mongo_ip}:{mongo_port}')

import pymongo
print('Connecting to mongodb... ')
mongo_client = pymongo.MongoClient(mongo_ip,mongo_port)
db = mongo_client['Battlecraft']
print('Connected')


# get key from environment variables
import os
key = os.getenv('BATTLECRAFT_JWT_KEY')
if key is None:
    print('Run the key setup script before running the server')
    exit(1)


import router
async def sendToRouter(websocket):
    message = await websocket.recv()
    messageObject = json.loads(message)
    requestType = messageObject['type']
    if requestType == 'user':
        await router.user(websocket,messageObject)
    elif requestType == 'player':
        await router.player(websocket,messageObject)
    elif requestType == 'world':
        await router.world(websocket,messageObject)
    else:
        await router.default(websocket,messageObject)

async def main():
    async with serve(sendToRouter,ip,port):
        await asyncio.get_running_loop().create_future()
