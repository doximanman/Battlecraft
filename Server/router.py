import controller.user as controller

async def user(websocket,request):
    subtype = request['subtype']
    if subtype == 'login':
        await controller.login(websocket,request)
    elif subtype == 'register':
        await controller.register(websocket,request)
    elif subtype == 'login_with_token':
        await controller.login_with_token(websocket,request)
    else:
        await controller.default(websocket,request)

async def player(websocket,request):
    return

async def world(websocket,request):
    return

async def default(websocket,request):
    return