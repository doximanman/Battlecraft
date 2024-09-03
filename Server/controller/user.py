import service.user as user
import json

async def login(websocket,request):
    username = request['username']
    password = request['password']

    success = await user.verify_user(username,password)

    if not success:
        await websocket.send(json.dumps({
            'success': False,
            'message': 'Incorrect credentials'
        }))
        return

    token = await user.gen_token(username,password)

    print(f'User logged in: {username}')

    await websocket.send(json.dumps({
        'success': True,
        'token': token
    }))

    return

async def login_with_token(websocket,request):
    token = request['token']

    check_user = await user.verify_with_token(token)
    if check_user['success']:
        print(f'User logged in: {check_user["username"]}')
    await websocket.send(json.dumps(check_user))

async def register(websocket,request):
    username = request['username']
    password = request['password']

    result = await user.create_user(username,password)

    print(f'User created: {username}')

    await websocket.send(json.dumps(result))

    return

async def default(websocket, request):
    await websocket.send('Unknown request type')
    return