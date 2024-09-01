import service.world as world
import json

async def get_data(websocket,request):
    # get token from request, get data using token,
    # send response.
    token = request['token']
    result = await world.get_data(token)

    await websocket.send(json.dumps(result))
    return

async def save_data(websocket,request):
    # get token and data from request,
    # save data using token and data,
    # send response.
    token = request['token']
    data = request['data']
    result = await world.save_data(token,data)
    await websocket.send(json.dumps(result))

    return

async def default(websocket,request):
    return