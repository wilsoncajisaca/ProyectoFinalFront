using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinal.Models
{
    public static class Common
    {
        public static string BaseUrl { get; set; } = "https://96a0-190-123-34-107.ngrok-free.app";
        public static string RoleUser { get; set; } = "USER";
        public static string RoleAdmin { get; set; } = "ADMIN";
        public static string STATUS_ACTIVE { get; set; } = "EN REVISION";
        public static string STATUS_DECLINE { get; set; } = "ANULADO";
    }
}
