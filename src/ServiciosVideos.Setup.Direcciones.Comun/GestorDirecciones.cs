using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class GestorDirecciones
    {
        private readonly IProveedorMarcadoresServidores _proveedorMarcadores;

        public GestorDirecciones(IProveedorMarcadoresServidores proveedorMarcadores)
        {
            _proveedorMarcadores = proveedorMarcadores;
        }

        public ResultadoActualizarDirecciones Actualizar(DescripcionServicio descripcionServicio, List<DireccionServidor> listaDireccionServidor, bool ignorarSiNoExistePlantilla = true, bool backup = true)
        {
            List<ActualizarDireccionServidorInput> listaDireccionServidorInput = new List<ActualizarDireccionServidorInput>();
            foreach( var direccion in listaDireccionServidor)
            {
                listaDireccionServidorInput.Add(new ActualizarDireccionServidorInput { NombreServidor = direccion.NombreServidor, DireccionIp = direccion.DireccionIp });

            }

            return Actualizar(descripcionServicio, listaDireccionServidorInput, ignorarSiNoExistePlantilla, backup);
        }

        public ResultadoActualizarDirecciones Actualizar(DescripcionServicio descripcionServicio, IList<ActualizarDireccionServidorInput> listaDireccionServidorInput, bool ignorarSiNoExistePlantilla = true, bool backup = true)
        {
            var plantilla = ObtenerNombreArchivoPlantilla(descripcionServicio);
            var existePlantila = File.Exists(plantilla);
            if (ignorarSiNoExistePlantilla && !existePlantila)
            {
                return new ResultadoActualizarDirecciones
                {
                    Success = true,
                    Observacion = "Sin Plantilla. Actualizacion no realizada"
                };
            }

            if (!existePlantila)
            {
                return new ResultadoActualizarDirecciones
                {
                    Success = false,
                    Error = $"No existe plantilla para el servicio. archivo:{plantilla}. servicio:{descripcionServicio.Nombre}",
                    Observacion = "Actualizacion Fallida. Plantilla no Encontrada"
                };
            }

            var porActualizar = new List<(ActualizarDireccionServidorInput direccionServidorInput, MarcadorServidor marcador)>();

            foreach (var direccionServidorInput in listaDireccionServidorInput)
            {

                var marcador = _proveedorMarcadores.ObtenerPorNombreServidor(direccionServidorInput.NombreServidor) ??
                    new MarcadorServidor
                    {
                        NombreServidor = direccionServidorInput.NombreServidor,
                        MarcadorDireccionIp = $"[[{direccionServidorInput.NombreServidor}_DireccionIp]]"
                    };

                porActualizar.Add((direccionServidorInput, marcador));
            }

            return  ActualizarSync(descripcionServicio, plantilla, porActualizar, backup);
        }

        private ResultadoActualizarDirecciones ActualizarSync(DescripcionServicio descripcionServicio, string plantilla, List<(ActualizarDireccionServidorInput direccionServidorInput, MarcadorServidor marcador)> porActualizar, bool backup)
        {
            try
            {
                string target = ObtenerDestino(descripcionServicio);

                TryBackup(target);

                var content = File.ReadAllText(plantilla);

                foreach (var actualizacion in porActualizar)
                {
                    content = content.Replace(actualizacion.marcador.MarcadorDireccionIp, actualizacion.direccionServidorInput.DireccionIp);
                }

                File.WriteAllText(target, content);              

                return new ResultadoActualizarDirecciones
                {
                    Success = true,

                    Observacion = "Actualizacion Exitosa"
                };
            }
            catch (Exception exception)
            {
                return new ResultadoActualizarDirecciones
                {
                    Success = false,
                    Error = exception.Message,
                    Exception = exception,
                    Observacion = "Actualizacion Fallida"
                };
            }
        }

        public async Task<ResultadoActualizarDirecciones> ActualizarAsync(DescripcionServicio descripcionServicio, IList<DireccionServidor> listaDireccionServidor, bool ignorarSiNoExistePlantilla = true, bool backup = true)
        {
            List<ActualizarDireccionServidorInput> listaDireccionServidorInput = new List<ActualizarDireccionServidorInput>();
            foreach (var direccion in listaDireccionServidor)
            {
                listaDireccionServidorInput.Add(new ActualizarDireccionServidorInput { NombreServidor = direccion.NombreServidor, DireccionIp = direccion.DireccionIp });

            }

            return await ActualizarAsync(descripcionServicio, listaDireccionServidorInput, ignorarSiNoExistePlantilla, backup);
        }

        public async Task<ResultadoActualizarDirecciones>  ActualizarAsync(DescripcionServicio descripcionServicio, IList<ActualizarDireccionServidorInput> listaDireccionServidorInput, bool ignorarSiNoExistePlantilla= true, bool backup=true)
        {

            var plantilla = ObtenerNombreArchivoPlantilla(descripcionServicio);
            var existePlantila = File.Exists(plantilla);
            if (ignorarSiNoExistePlantilla && !existePlantila)
            {
                return new ResultadoActualizarDirecciones
                {
                    Success = true,
                    Observacion="Sin Plantilla. Actualizacion no realizada"
                };
            }

            if (!existePlantila)
            {
                return new ResultadoActualizarDirecciones
                {
                    Success = false,
                    Error = $"No existe plantilla para el servicio. archivo:{plantilla}. servicio:{descripcionServicio.Nombre}",
                    Observacion ="Actualizacion Fallida. Plantilla no Encontrada"
                };
            }

            var porActualizar = new List<(ActualizarDireccionServidorInput direccionServidorInput, MarcadorServidor marcador )>();

            foreach(var direccionServidorInput in listaDireccionServidorInput)
            {

                var marcador = await _proveedorMarcadores.ObtenerPorNombreServidorAsync(direccionServidorInput.NombreServidor) ??
                    new MarcadorServidor
                    {
                        NombreServidor = direccionServidorInput.NombreServidor,
                        MarcadorDireccionIp = $"[[{direccionServidorInput.NombreServidor}_DireccionIp]]"
                    };

                porActualizar.Add((direccionServidorInput, marcador));
            }

            return await ActualizarAsync(descripcionServicio, plantilla, porActualizar, backup);

        }


        private async Task<ResultadoActualizarDirecciones> ActualizarAsync(DescripcionServicio descripcionServicio, string plantilla, List<(ActualizarDireccionServidorInput direccionServidorInput, MarcadorServidor marcador)> porActualizar, bool backup)
        {
            try
            {
                string target = ObtenerDestino(descripcionServicio);

                TryBackup(target);

                var content = await plantilla.ReadAllTextAsync();

                foreach( var actualizacion in porActualizar)
                {
                    content = content.Replace(actualizacion.marcador.MarcadorDireccionIp, actualizacion.direccionServidorInput.DireccionIp);
                }

                await target.WriteAllTextAsync(content);

                return new ResultadoActualizarDirecciones
                {
                    Success = true,

                    Observacion = "Actualizacion Exitosa"
                };
            } 
            catch(Exception exception)
            {
                return new ResultadoActualizarDirecciones
                {
                    Success = false,
                    Error = exception.Message,
                    Exception = exception,
                    Observacion = "Actualizacion Fallida"
                };
            }
        }

        private bool TryBackup(string filename)
        {
            try
            {
                if (!File.Exists(filename)) return true;
                var backupfile = $"{filename}-{DateTimeExtensions.NowToFilenameString()}.backup";
                File.Copy(filename, backupfile, overwrite: true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string ObtenerDestino(DescripcionServicio descripcionServicio)
        {
            return Path.Combine(descripcionServicio.DirectorioDespliegue, descripcionServicio.SettingsFile);
        }

        private static string ObtenerNombreArchivoPlantilla(DescripcionServicio descripcionServicio)
        {
            var plantilla = !string.IsNullOrEmpty(descripcionServicio.SettingsTemplate) ? descripcionServicio.SettingsTemplate : $"{descripcionServicio.SettingsFile}.template";

            return Path.Combine( descripcionServicio.DirectorioDespliegue, plantilla);
        }
    }

    public class ResultadoActualizarDirecciones
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public Exception Exception { get; set; }
        public string  Observacion { get; set; }

    }
}
