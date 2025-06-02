TODO

from: https://dev.to/clifftech123/learn-minimal-apis-in-net-8-full-crud-tutorial-for-beginners-1b86

DOCKER COMMANDS

docker build --no-cache -t fc_minimal_api_image:1.0 -f Dockerfile .

Dev
docker run --restart always --name fc_minimal_api -d -p 30119:8080 -v /Users/luca/GitHub/FC_API/FC_API/Temp:/data fc_minimal_api_image:1.0

docker run --restart always --name oneapp_api -d -p 30119:8080 -v /Users/luca/GitHub/FC_API/FC_API/Temp:/data oneapp_api:latest

Prod
docker run --restart always --name fc_minimal_api -d -p 30109:8080 -v /share/CACHEDEV2_DATA/Storage/Docker/file_categorization:/data -v /share/Download/Incoming:/incoming -v /share/Video/Serie:/serie fc_minimal_api_image:1.0
 

Secrets/Environment Variables:

//Dev:
//dotnet user-secrets init
//dotnet user-secrets set "JWT:SECRET" "12345"
//dotnet user-secrets set "CRYPTO:MasterKey" "12345"

//Prod:
//-e "JWT_SECRET=12345" \
//-e "CRYPTO_MASTERKEY=12345" 

SECRET ON LOCAL VAULT:
add MASTERKEY_SECRET:
dotnet user-secrets set "MASTERKEY_SECRET" "12345" (dev)
... -e "MASTERKEY_SECRET=12345" (prod)

Add to local db:
JWT:SECRET = 12345
CRYPTO:MASTERKEY = 12345