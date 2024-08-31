import service.player as player
import json

async def get_data(websocket,request):
    # get token from request, get data using token,
    # send response.
    token = request['token']
    result = await player.get_data(token)
    await websocket.send(json.dumps(result))
    return

async def save_data(websocket,request):
    # get token and data from request,
    # save data using token and data,
    # send response.
    token = request['token']
    data = request['data']
    result = await player.save_data(token,data)
    await websocket.send(json.dumps(result))

    return

async def default(websocket,request):
    return