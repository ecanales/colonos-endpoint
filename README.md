# colonos-endpoint
Colonos EndPoint

#usar init cuando es un espacio de trabajo nuevo
git init

#para mantener siempre la ultima versión del repo ------
#ubicarse en main local
git branch -M main
#conectar a repositorio remoto
git remote add origin https://github.com/ecanales/colonos-endpoint.git
#recuperar cambios existentes en repo remoto
git pull origin main
git status
#-------------------------------------------------------
#subir cambios o modificaciones ------------------------
#ubicarse en main local
git branch -M main
#conectar a repositorio remoto
git remote add origin https://github.com/ecanales/colonos-endpoint.git
git status
#si hay cambios se listaran en rojo 
git add .
git status
#confirmar cambios
git commit -m "first commit"
git push -u origin main
git status
#-------------------------------------------------------
#Fin
