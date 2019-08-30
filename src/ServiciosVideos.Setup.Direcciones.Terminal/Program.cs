using Terminal.Gui;
using System;
using Ci2.Comun.SystemTools;
using System.Threading.Tasks;
using System.Linq;
using ServiciosVideos.Setup.Direcciones.Comun;
using System.Collections.Generic;


namespace ServiciosVideos.Setup.Direcciones.TerminalGui
{
    class Program
    {
        static void Main(string[] args)
        {

            Application.Init();


            var top = Application.Top;

            var tframe = top.Frame;

            var win = new Window("Direcciones Servidores")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Salir", "", () => { if (Quit ()) top.Running = false; })
                })
            });

            MostrarPantallaDirecciones(win, top);

            top.Add(win);
            top.Add(menu);
            Application.Run();

        }

        static void MostrarPantallaDirecciones(View container, Toplevel top)
        {

            var proveedorTarjetaRed = new JsonProveedorTarjetaRed();

            NetworkInteface tarjetaRedAlmacenada = proveedorTarjetaRed.Obtener();

            var proveedorServidores = new JsonProveedorDireccionesServidores();

            var whoami = Bash.WhoAmI();
            var listaServidores = proveedorServidores.Consultar();

            var equipoActual = listaServidores.FirstOrDefault(q => q.NombreServidor.ToUpper() == whoami.ToUpper()) ?? new DireccionServidor { NombreServidor = whoami.UppercaseWords(), DireccionIp = "" };

            listaServidores.RemoveAll(q => q.NombreServidor.ToUpper() == equipoActual.NombreServidor.ToUpper()); // servidores excepto equipo actual

            var tarjetasRedDisponibles = (Network.GetNetworkInterfaces())
                .Where(q => q.CardName != Network.loopback).ToList();

            var indiceTarjetaRedSeleccionada = tarjetasRedDisponibles.FindIndex(q => q.CardName == tarjetaRedAlmacenada.CardName);
            if (indiceTarjetaRedSeleccionada < 0) indiceTarjetaRedSeleccionada = 0;

            // tarjetas de red disponible
            var etiquetasTarjetasRedDisponibles = tarjetasRedDisponibles.ConvertAll(q => q.ToString()).ToArray();
            var radioGroupTarjetaRedDisponibles = new RadioGroup(etiquetasTarjetasRedDisponibles) { X = 1, Y = 0 };

            var frameTarjetas = new FrameView($"Seleccione la Tarjeta de Red a utilizar para el equipo: {equipoActual.NombreServidor}")
            {
                X = 1,
                Y = 1,
                Width = 110,
                Height = etiquetasTarjetasRedDisponibles.Length + 3
            };

            frameTarjetas.Add(radioGroupTarjetaRedDisponibles);


            // si la tarjeta almacenada no cotiene informacion llenarla con info de la tarjeta red seleccionada inicial..
            tarjetaRedAlmacenada.Inet = !string.IsNullOrEmpty(tarjetaRedAlmacenada.Inet) ? tarjetaRedAlmacenada.Inet : tarjetasRedDisponibles[indiceTarjetaRedSeleccionada].Inet;
            tarjetaRedAlmacenada.Netmask = !string.IsNullOrEmpty(tarjetaRedAlmacenada.Netmask) ? tarjetaRedAlmacenada.Netmask : tarjetasRedDisponibles[indiceTarjetaRedSeleccionada].Netmask;

            // equipo actual la direccion para el equipo actual debe ser el valor de la tarjetaAlmacenada !!!
            equipoActual.DireccionIp = tarjetaRedAlmacenada.Inet;

            var labelEquipoActual = new Label($"Direccion Ip") { X = 1, Y = 1, Width = 18, TextAlignment = TextAlignment.Left };
            var textDireccionIpEquipoActual = new TextField(equipoActual.DireccionIp) { X = 24 + 1, Y = 1, Width = 20, Used = true };
            var labelErrorDireccionIpEquipoActual = new Label($"") { X = 20 + 20 + 6, Y = 1, Width = 1 };

            var labelMascara = new Label($"Mascara (1-32)") { X = 52, Y = 1, Width = 18, TextAlignment = TextAlignment.Left };
            var textMascaraTarjetaRedActual = new TextField(tarjetaRedAlmacenada.CidrBits) { X = 71, Y = 1, Width = 4, Used = true };
            var labelMascaraDecimalEquipoActual = new Label($"{tarjetaRedAlmacenada.Netmask}") { X = 80, Y = 1, Width = 20 };

            var labelGateway = new Label($"Gateway") { X = 1, Y = 2, Width = 18 };
            var textGatewayTarjetaActual = new TextField(tarjetaRedAlmacenada.Gateway) { X = 24 + 1, Y = 2, Width = 20, Used = true };

            var labelDns = new Label($"DNS") { X = 52, Y = 2, Width = 18 };
            var textDnsTarjetaRedActual = new TextField(tarjetaRedAlmacenada.NameServersAddresses) { X = 71, Y = 2, Width = 30, Used = true };

            var frameEquipoActual = new FrameView(new Rect(1, etiquetasTarjetasRedDisponibles.Length + 4, 110, 6), $"Tarjeta de Servidor local: {equipoActual.NombreServidor}");
            frameEquipoActual.Add(labelEquipoActual, textDireccionIpEquipoActual, labelErrorDireccionIpEquipoActual,
                labelMascara, textMascaraTarjetaRedActual, labelMascaraDecimalEquipoActual, labelGateway, textGatewayTarjetaActual, labelDns, textDnsTarjetaRedActual);

            // info errores en los datos
            var labelErrores = new Label(1, etiquetasTarjetasRedDisponibles.Length + 4 + 6 + listaServidores.Count + 4 + 4, "");

            //
            // servidores
            var listLabelServidores = new List<Label>();
            var listTextServidores = new List<TextField>();
            var y = 1;
            listaServidores.ForEach(servidor =>
            {
                var lbl = new Label(servidor.NombreServidor) { X = 1, Y = y, TextAlignment = TextAlignment.Left, Width = 18 };

                listLabelServidores.Add(lbl);
                var txtfield = new TextField(servidor.DireccionIp) { X = 24 + 1, Y = y++, Width = 20, Used = true };

                txtfield.Changed += (evt, obj) =>
                {
                    var value = txtfield.Text.ToString().Trim();
                    servidor.DireccionIp = value;
                    UpdateLabel(labelErrores, Network.IsValidIpAddress(value), $"Direccion Ip no Vlida en '{servidor.NombreServidor}'");
                };
                listTextServidores.Add(txtfield);

            });

            var frameServidores = new FrameView(new Rect(1, etiquetasTarjetasRedDisponibles.Length + 4 + 6, 110, listaServidores.Count + 4), $"Direccion IP Servidores");
            frameServidores.Add(listLabelServidores.ToArray());
            frameServidores.Add(listTextServidores.ToArray());
            //

            // clave sudo
            var labelPassword = new Label($"Digite su clave") { X = 1, Y = 1, Width = 18, TextAlignment = TextAlignment.Left };
            var textPassword = new TextField("") { X = 24 + 1, Y = 1, Width = 20, Used = true, Secret = true };
            var testSudoBtn = new Button(52, 1, "Verificar");
            var testSudoLabel = new Label("") { X = 65, Y = 1 };
            var framePassword = new FrameView(new Rect(1, etiquetasTarjetasRedDisponibles.Length + 4 + 6 + listaServidores.Count + 4, 110, 4), $"Clave de usuario");
            framePassword.Add(labelPassword, textPassword, testSudoBtn, testSudoLabel);
            //
                                    


            // Guardar
            var guardarBtn = new Button(1, 1, "Guardar");
            var salirBtn = new Button(20, 1, " Salir");
            var frameGuardarSalir = new FrameView(new Rect(1, etiquetasTarjetasRedDisponibles.Length + 4 + 6 + listaServidores.Count + 4 + 5, 110, 4), "");
            frameGuardarSalir.Add(guardarBtn, salirBtn);
            //


            // label para guardando.....
            var labelGuardar = new Label(1, etiquetasTarjetasRedDisponibles.Length + 4 + 6 + listaServidores.Count + 4 + 5 + 4, "");
            //
            
            // list view para resultados actualizacion  direcciones en los servicios;
            var resultadosActualizacionDireccionesListView = new ListView(new Rect(1, etiquetasTarjetasRedDisponibles.Length + 4 + 6 + listaServidores.Count + 4 + 5 + 6, 110, 4), new string[] {

            })
            { CanFocus = false };

            
            container.Add(frameTarjetas, frameEquipoActual, frameServidores, framePassword, labelErrores, frameGuardarSalir, labelGuardar, resultadosActualizacionDireccionesListView);


            /// Manejadores de eventos 
            radioGroupTarjetaRedDisponibles.SelectionChanged = (selected) =>
            {
                frameEquipoActual.Title = $"Tarjeta de Red Servidor Local: {equipoActual.NombreServidor} en {tarjetasRedDisponibles[selected].CardName}";
                tarjetaRedAlmacenada.CardName = tarjetasRedDisponibles[selected].CardName;
            };
            radioGroupTarjetaRedDisponibles.Selected = indiceTarjetaRedSeleccionada;


            textDireccionIpEquipoActual.Changed += (obj, evt) =>
            {
                var value = textDireccionIpEquipoActual.Text.ToString().Trim();
                var isvalidValue = Network.IsValidIpAddress(value);
                tarjetaRedAlmacenada.Inet = value;
                equipoActual.DireccionIp = value;
                labelErrorDireccionIpEquipoActual.Text = isvalidValue ? "" : "X";
                UpdateLabel(labelErrores, isvalidValue,  "Direccion Ip Servidor local no valida");
            };


            textMascaraTarjetaRedActual.Changed += (obj, evt) =>
            {
                var value = textMascaraTarjetaRedActual.Text.ToString().Trim();
                var netmask = MascarasRed.ObtenerNotacionDecimal(value);
                labelMascaraDecimalEquipoActual.Text = netmask;
                tarjetaRedAlmacenada.Netmask = netmask;
                UpdateLabel(labelErrores, Network.IsValidCidr(value), "Mascara Servidor local no  valida");
            };
            

            textGatewayTarjetaActual.Changed += (obj, evt) =>
            {
                var value = textGatewayTarjetaActual.Text.ToString().Trim();
                tarjetaRedAlmacenada.Gateway = value;
                UpdateLabel(labelErrores, Network.IsValidIpAddress(value), "Gateway Servidor local no  valido");
            };


            textDnsTarjetaRedActual.Changed += (obj, evt) =>
            {
                var value = string.Join(',', textDnsTarjetaRedActual.Text.ToString().Trim().Replace(" ", "").Split(','));
                tarjetaRedAlmacenada.NameServersAddresses = value;
                UpdateLabel(labelErrores, IsValidNameServersAddresses(value), "DNS Servidor local no  valido");
            };


            testSudoBtn.Clicked = () => _= TestSudoButttonClicked(textPassword, testSudoLabel);
            

            textPassword.Changed += (obj, evt) =>
            {
                testSudoLabel.Text = "".PadRight(30, ' ');
            };


            salirBtn.Clicked = () =>
            {
                if (Quit()) top.Running = false;
            };


            guardarBtn.Clicked = () => _ = GuadarBtnClicked(tarjetaRedAlmacenada, listaServidores, equipoActual, textPassword, labelGuardar, testSudoLabel, resultadosActualizacionDireccionesListView);
            
        }
        

        private static async Task TestSudoButttonClicked(TextField textPassword, Label testSudoLabel)
        {
            testSudoLabel.Text = "Verificando...".PadRight(30, ' ');
            await Task.Run(async () =>
            {
                var isvalid = await ValidatePassword(textPassword);
                Application.MainLoop.Invoke(() => testSudoLabel.Text = isvalid ? "OK".PadRight(30, ' ') : "usuario/clave incorrectos".PadRight(30, ' '));
            });
        }


        private static async Task GuadarBtnClicked(NetworkInteface tarjetaRedAlmacenada, List<DireccionServidor> listaServidores, DireccionServidor equipoActual, TextField textPassword, Label labelGuardar,
            Label testSudoLabel, ListView resultadoActualizacion)
        {
            ActualizarLabelGuardar(labelGuardar, "", enMainLoop: false);

            resultadoActualizacion.SetSource(new List<string>());

            // datos para guardar en NetworkInterface
            if (!ValidateNetworkInterface(tarjetaRedAlmacenada))
            {
                return;
            }

            // validar ips en lista servidores
            if (!ValidateServers(listaServidores))
            {
                return;
            }


            await Task.Run(async () =>
            {

                testSudoLabel.Text = "Verificando...".PadRight(30, ' ');

                var _isvalid = await ValidatePassword(textPassword);

                Application.MainLoop.Invoke(() =>
                {
                    testSudoLabel.Text = _isvalid ? "OK".PadRight(30, ' ') : "usuario/clave incorrectos".PadRight(30, ' ');
                    if (!_isvalid) ShowError("usuario/clave incorrectos");
                });

                if (!_isvalid) return;


                await SaveNetworInterfaceAsync(tarjetaRedAlmacenada, labelGuardar);
                await GuardarDireccionesServidoresAsync(listaServidores, equipoActual, tarjetaRedAlmacenada, labelGuardar);
                await ActualizarConfiguracionServicios(listaServidores, resultadoActualizacion);

                if (!(await CrearArchivoNetplan(tarjetaRedAlmacenada, labelGuardar, textPassword.Text.ToString().Trim())))
                {
                    return;
                }


                if (await NetplanApply(textPassword.Text.ToString().Trim(), labelGuardar))
                {
                    ActualizarLabelGuardar(labelGuardar, "Actualizacion finalizada con EXITO");
                }

            });
        }


        private static async Task<bool> NetplanApply(string password, Label labelGuardar)
        {
            ActualizarLabelGuardar(labelGuardar, $"aplicando netplan");
            
            var result = await Netplan.ApplyAsync(password);
            if(result.ExitCode==0)
            {
                ActualizarLabelGuardar(labelGuardar, $"aplicado netplan con exito");
                return true;
            }
            else
            {
                ActualizarLabelGuardar(labelGuardar, $"netplan : {result.LastOutput}");
                return false;
            }
        }


        private static async Task<bool> CrearArchivoNetplan(NetworkInteface tarjetaRedAlmacenada, Label labelGuardar, string password)
        {
            ActualizarLabelGuardar(labelGuardar, $"Creando  ...  archivo netplan");
            
            var result = await Netplan.CreateYamlAsync(tarjetaRedAlmacenada.CardName, tarjetaRedAlmacenada.Inet, tarjetaRedAlmacenada.CidrBits, tarjetaRedAlmacenada.Gateway,  tarjetaRedAlmacenada.NameServersAddresses, password:password);
            if (result.ExitCode == 0)
            {
                ActualizarLabelGuardar(labelGuardar, $"Creado  archivo netplan {tarjetaRedAlmacenada.CardName}:{tarjetaRedAlmacenada.Inet}/{tarjetaRedAlmacenada.CidrBits} {result.LastOutput} ");                
                return true;
            }
            else
            {
                ActualizarLabelGuardar(labelGuardar, $"crear archivo netplan {result.LastOutput}");                
                return false;
            }
        }


        private static async Task SaveNetworInterfaceAsync(NetworkInteface tarjetaRedAlmacenada, Label labelGuardar)
        {
            ActualizarLabelGuardar(labelGuardar, $"Guardando ... {tarjetaRedAlmacenada.GetType().Name}");
            var proveedor = new JsonProveedorTarjetaRed();
            await proveedor.GuardarAsync(tarjetaRedAlmacenada);
            ActualizarLabelGuardar(labelGuardar, $"Guardado      {tarjetaRedAlmacenada.GetType().Name}");
        }


        private static async Task GuardarDireccionesServidoresAsync(List<DireccionServidor> listaServidores, DireccionServidor equipoActual, NetworkInteface tarjetaRedAlmacenada,  Label labelGuardar)
        {
            var direccionesServidoresPorActualizar = new List<DireccionServidor>();
            direccionesServidoresPorActualizar.AddRange(listaServidores);
            direccionesServidoresPorActualizar.Add(new DireccionServidor { NombreServidor = equipoActual.NombreServidor, DireccionIp = tarjetaRedAlmacenada.Inet });

            await Task.Delay(1000);
            ActualizarLabelGuardar(labelGuardar, $"Guardando ... Lista con Direcciones de los servidores");
            var proveedor = new JsonProveedorDireccionesServidores();
            await proveedor.GuardarAsync(direccionesServidoresPorActualizar);
            await Task.Delay(1000);
            ActualizarLabelGuardar(labelGuardar, $"Guardado      Lista con Direcciones de los servidores");
        }


        private static async Task ActualizarConfiguracionServicios(List<DireccionServidor> listaServidores, ListView resultadosActualizacionLisView  )
        {
            
            Application.MainLoop.Invoke(() => resultadosActualizacionLisView.SetSource(new List<string>()));

            var proveedorMarcadores = new JsonProveedorMarcadoresServidores();
            var gestorDirecciones = new GestorDirecciones(proveedorMarcadores);
            var proveedorServicios = new JsonProveedorDescripcionesServicios();
            var servicios = await proveedorServicios.ConsultarAsync();
            var respuestaActualizacionConfiguracion = new ListaRespuestaActualizarConfiguracionServicio();

            foreach (var servicio in servicios)
            {
                var resultadoActualizacion = await gestorDirecciones.ActualizarAsync(servicio, listaServidores, ignorarSiNoExistePlantilla: true, backup: true);
                respuestaActualizacionConfiguracion.Add(new RespuestaActualizarConfiguracionServicio { Servicio = servicio, ResultadoActualizacion = resultadoActualizacion });
                await Task.Delay(1000);
                var listviewSource = respuestaActualizacionConfiguracion.Items.ConvertAll(q => $"{q.Servicio.Nombre.PadRight(20)} : {q.ResultadoActualizacion.Observacion}").ToArray();
                Application.MainLoop.Invoke(() => resultadosActualizacionLisView.SetSource(listviewSource));
            }
                        
            try
            {
                await respuestaActualizacionConfiguracion.SaveToFileAsync();
            }
            catch (Exception)
            {

            }

        }

                


        private static async Task<bool> ValidatePassword(TextField textPasword)
            => (!textPasword.Text.IsEmpty && (await Bash.IsSudoPasswordAsync(textPasword.Text.ToString().Trim())));
                   
                

        private static void UpdateLabel(Label labelErrores, bool isvalidValue, string noValidMessage, string validMessage="")
        {
            labelErrores.Text = isvalidValue ? validMessage.PadRight(50, ' '): noValidMessage.PadRight(50, ' ');
        }

        private static bool ValidateServers(List<DireccionServidor> listaServidores)
        {
            foreach (var servidor in listaServidores)
            {
                if (!Network.IsValidIpAddress(servidor.DireccionIp))
                {
                    ShowError($"direcion Ip {servidor.DireccionIp} no Valida en {servidor.NombreServidor} ");
                    return false;
                }
            }
            return true;
        }
        

        private static bool ValidateNetworkInterface(NetworkInteface tarjetaRedAlmacenada)
        {
            if (!Network.IsValidIpAddress(tarjetaRedAlmacenada.Inet))
            {
                ShowError($"Direccion IP no valida en Tarjeta de Red Servidor Local: '{tarjetaRedAlmacenada.Inet}'");
                return false;
            }

            if (!Network.IsValidCidr(tarjetaRedAlmacenada.CidrBits))
            {
                ShowError($"Mascara de red no valida en Tarjeta de Red Servidor Local '{tarjetaRedAlmacenada.CidrBits}'");
                return false;
            }

            if (!Network.IsValidIpAddress(tarjetaRedAlmacenada.Gateway))
            {
                ShowError($"Gateway no valido en Tarjeta de Red Servidor Local :'{tarjetaRedAlmacenada.Gateway}'");
                return false;
            }

            if(!IsValidNameServersAddresses(tarjetaRedAlmacenada.NameServersAddresses))
            {
                ShowError($"Direccion(es) DNS no valida(s) en Tarjeta de Red Servidor Local :'{tarjetaRedAlmacenada.NameServersAddresses}'");
                return false;
            }

            return true;
        }

        private static void ActualizarLabelGuardar(Label labelGuardar, string mensaje, bool enMainLoop = true)
        {
            if (enMainLoop)
            {
                Application.MainLoop.Invoke(() => labelGuardar.Text = mensaje.PadRight(180));
            }
            else
            {
                labelGuardar.Text = mensaje.PadRight(180);
            }
        }

        private static bool IsValidNameServersAddresses(string nameServersAddresses)
        {
            if (string.IsNullOrEmpty(nameServersAddresses)) return true;
            var addresses = nameServersAddresses.Split(',');
            foreach( var address in addresses)
            {
                if (! Network.IsValidIpAddress(address)) return false;
            }
            return true;
        }

        static void ShowError(string error)
        {
            MessageBox.ErrorQuery(80, 5, "Error", error, "Ok");
        }


        static bool Quit()
        {
            return true;
            var n = MessageBox.Query(50, 7, "Salir", "Desea salir?", "Si", "No");
            return n == 0;
        }


    }
}

