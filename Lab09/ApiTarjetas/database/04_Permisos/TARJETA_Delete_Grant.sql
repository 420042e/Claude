/*************************************************************
*Descripcion        : Permiso granular de ejecución de [api].[TARJETA_Delete]
*                     para el usuario de la aplicación. Idempotente.
*                     GATE 4 (CLAUDE.md): lo revisa y aplica un humano.
*Fecha Crea         : 2026-10-13
*Fecha Mod          :
*Parametros         : (ninguno)
*Autor              : Equipo Tarjetas
*Version            : 1.0
*Cambios Importantes: HU-TAR-07
**************************************************************/
IF DATABASE_PRINCIPAL_ID('UsrBD_ApiTarjetas') IS NOT NULL
    GRANT EXECUTE ON [api].[TARJETA_Delete] TO [UsrBD_ApiTarjetas];
GO
