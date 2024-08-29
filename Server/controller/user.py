from server import key
import service.user as user
import jwt
from hashlib import sha256
import json

async def login(websocket,request):
    username = request['username']
    password = request['password']

    success = user.verify_user(username,password)

    if not success:
        resultJson = {
            'success': False,
            'message': 'Incorrect credentials'
        }
        websocket.send(json.dumps(resultJson))
        return

    passwordHash = sha256(password.encode()).hexdigest()

    # create JWT token
    token = jwt.encode({
        'username': username,
        'passwordHash': passwordHash,
    },key,algorithm='HS256')



    return

async def register(websocket,request):
    username = request['username']
    password = request['password']

    return

async def default(websocket, request):
    await websocket.send('Unknown request type')
    return