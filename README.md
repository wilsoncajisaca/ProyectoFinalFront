URL PARA ACCEDER A LOS SERVICIOS WEB.
https://96a0-190-123-34-107.ngrok-free.app

TIPO PETICION: "POST"
*REGISTRO DE UN USUARIO*
/appMovilesFinal/api/partner/create
Nota: Genero 0:Hombre 1: mujer
{
    "cedula":"",
    "nombre":"",
    "apellido":"",
    "direccion":"a",
    "email":"",
    "contrasenia":"",
    "celular":"",
    "genero":"",
    "isAdmin":true|false
}

TIPO PETICION: "POST"
*LOGIN DE UN USUARIO*
/appMovilesFinal/api/auth/login
Nota: username: Es el numero de cedula.
{
    "username":"",
    "password":"" 
}

TIPO PETICION: "POST"
*CREACION DE UN SINIESTRO*
/appMovilesFinal/api/sinister/create
Nota: La imagen debe ser convertida a base64 y se debe enviar tambien la extencion del archivo. (ejemplo: png, jpg, etc...)
{
    "fileBase64": "",
    "extensionArchivo":"",
    "ubicacionSiniestro": "0115115151555,-1185518511515",
    "tipoSiniestro": "61e946ab-c4d7-4348-b09f-fb874bb91db8",
    "descripcion": "Se produjo un rompimiento de tuveria"
}

TIPO PETICION: "GET"
*OBTENER LA INFORMACION DE UN SINIESTRO EN ESPECIFICO* Se debe adjuntar el id del siniestro.
/appMovilesFinal/api/sinister/getSinister?sinisterId=XXX
Remplazar "XXX" por el valor id del siniestro.

TIPO PETICION: "GET"
*OBTIENE TODOS LOS SINIESTROS DEL USUARIO LOGEADO*
/appMovilesFinal/api/sinister/getAllSinisterByPartner

TIPO PETICION: "GET"
*OBTIENE TODOS LOS SINIESTROS* En este caso debe ser tipo administrador.
/appMovilesFinal/api/sinister/getAllSinister

TIPO PETICION: "GET"
*OBTIENE TODOS LOS TIPOS DE SINIESTROS*
/appMovilesFinal/api/sinisterType/getAllSinisterType
