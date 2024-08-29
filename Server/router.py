import controller

async def user(websocket,request):
    subtype = request['subtype']
    if subtype == 'login':
        await controller.user.login(websocket,request)
    elif subtype == 'register':
        await controller.user.register(websocket,request)
    else:
        await controller.user.default(websocket,request)

async def player(websocket,request):
    return

async def world(websocket,request):
    return

async def default(websocket,request):
    return