from globals import db
import service.token as token_service
import service.user as user_service

players_db = db['players']

async def get_data(token: str):
    preUser = token_service.verify_token(token)
    if preUser['success']:
        user = preUser['data']
    else:
        return{
            'success': False,
            'message': preUser['message']
        }

    username = user['username']

    # make sure user exists
    user_exists = await user_service.verify_with_token(token)
    if not user_exists['success']:
        return{
            'success': False,
            'message': user_exists['message']
        }

    # check if user has data
    data = await players_db.find_one({'username': username})

    if not data:
        # no data but user exists
        return{
            'success': True,
            'data': ''
        }
    else:
        return{
            'success': True,
            'data': data['data']
        }


async def has_data(token: str):
    preUser = token_service.verify_token(token)
    if preUser['success']:
        user = preUser['data']
    else:
        return False

    data = await players_db.find_one({'username': user['username']})
    return data is not None


# note: data is a string, NOT a dictionary.
async def save_data(token: str, data: str):
    preUser = token_service.verify_token(token)
    if preUser['success']:
        user = preUser['data']
    else:
        return {
            'success': False,
            'message': preUser['message']
        }

    username = user['username']

    # remove data that exists
    if await has_data(token):
        await players_db.delete_one({'username': username})

    await players_db.insert_one({'username': username, 'data': data})
