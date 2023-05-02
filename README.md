# dotnetcore

create a file named .env\
`DB_HOST=Sever=${HOST};PORT=${PORT};User Id=${pg user};Password=${pwd};Database=${db name}`\
optional `PORT=${port}`\
`AUTH0_DOMAIN=${Domain}`

## Special

the login url is:\
`https://dev-1ufedzasg8yhi2gf.us.auth0.com/authorize?response_type=token&client_id=EwmJJjieu3vjF0AF4Smg8PSGXLeLd7vN&redirect_uri=https://yufengli.vercel.app&state=AUTHORIZED&audience=http://0.0.0.0:3000&scope=openid+profile+email`

## Migration

'''
dotnet ef migrations add {migration_name}
dotnet ef database update
'''
