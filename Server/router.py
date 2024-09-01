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


import controller.world as world_controller
async def world(websocket,request):
    subtype = request['subtype']
    if subtype == 'get':
        await world_controller.get_data(websocket,request)
    if subtype == 'save':
        await world_controller.save_data(websocket,request)
    else:
        await world_controller.default(websocket,request)
    return


async def default(websocket,request):
    return