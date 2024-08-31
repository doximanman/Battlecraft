import controller.user as user_controller


async def user(websocket,request):
    subtype = request['subtype']
    if subtype == 'login':
        await user_controller.login(websocket,request)
    elif subtype == 'register':
        await user_controller.register(websocket,request)
    elif subtype == 'login_with_token':
        await user_controller.login_with_token(websocket,request)
    else:
        await user_controller.default(websocket,request)


import controller.player as player_controller
async def player(websocket,request):
    subtype = request['subtype']
    if subtype == 'get_data':
        await player_controller.get_data(websocket,request)
    if subtype == 'save_data':
        await player_controller.save_data(websocket,request)
    else:
        await player_controller.default(websocket,request)
    return


async def world(websocket,request):
    return

async def default(websocket,request):
    return