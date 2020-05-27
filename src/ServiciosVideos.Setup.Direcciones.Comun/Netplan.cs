using System;
using System.IO;
using System.Threading.Tasks;

namespace ServiciosVideos.Setup.Direcciones.Comun
{
    public class Netplan
    {
        public static readonly string YAML_FILE = "/etc/netplan/50-cloud-init.yaml";
        
        public static async Task<CommandResult> ApplyAsync(string password)
        {
            return await Bash.ExecuteAsync("netplan apply", password);
        }
        
        /// <summary>
        /// Crea archivo /etc/netplan/50-cloud-init.yam"
        /// </summary>
        /// <param name="networkInterfaceName"></param>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <param name="gateway"></param>
        /// <param name="nameServersAddresses"></param>
        /// <returns></returns>
        public static async Task<CommandResult> CreateYamlAsync(string networkInterfaceName, string ip, string mask, string gateway, string nameServersAddresses, string password, bool backup=true)
        {

            var content = GetYamlTemplate(networkInterfaceName, ip, mask, gateway, nameServersAddresses);

            var outputfile =  YAML_FILE;
            
            var currentdir = Directory.GetCurrentDirectory();
            var tmpfile = Path.Combine(currentdir, System.IO.Path.GetFileName(outputfile));

            await tmpfile.WriteAllTextAsync(content);
            await Bash.ExecuteAsync($"chmod 640 {tmpfile}");

            if( backup && File.Exists(YAML_FILE))
            {
                var backupfilename = $"{YAML_FILE}-{DateTimeExtensions.NowToFilenameString()}.backup";
                await Bash.ExecuteAsync($"cp {YAML_FILE} {backupfilename}", password: password);
                
            }
                                    
            var result = await Bash.ExecuteAsync($"mv {tmpfile} {outputfile}", password: password);
            await Bash.ExecuteAsync($"chmod 640 {outputfile}");
            return result;
        }

        public static CommandResult CreateYaml(string networkInterfaceName, string ip, string mask, string gateway, string nameServersAddresses, string password, bool backup = true)
        {

            var content = GetYamlTemplate(networkInterfaceName, ip, mask, gateway, nameServersAddresses);

            var outputfile = YAML_FILE;

            var currentdir = Directory.GetCurrentDirectory();
            var tmpfile = Path.Combine(currentdir, System.IO.Path.GetFileName(outputfile));

            File.WriteAllText(tmpfile, content); 
            Bash.Execute($"chmod 640 {tmpfile}");

            if (backup && File.Exists(YAML_FILE))
            {
                var backupfilename = $"{YAML_FILE}-{DateTimeExtensions.NowToFilenameString()}.backup";
                Bash.Execute($"cp {YAML_FILE} {backupfilename}", password: password);
            }

            var result = Bash.Execute($"mv {tmpfile} {outputfile}", password: password);
            Bash.Execute($"chmod 640 {outputfile}");
            return result;
        }


        private static string GetYamlTemplate(string networkInterfaceName, string ip, string mask, string gateway, string nameServersAddresses)
        {
            return $@"network:
    ethernets:
        {networkInterfaceName}:
            addresses: [{ip}/{mask}]
            gateway4: {gateway}
            nameservers:
              addresses: [{nameServersAddresses}]
            dhcp4: no
    version: 2"; }
    }
}

/*
network:
    ethernets:
        enp3s0:
            addresses: [172.16.2.131/24]
gateway4: 172.16.2.1
            nameservers:
              addresses: [8.8.8.8,8.8.4.4]
dhcp4: no
version: 2
*/
//var bash = new Bash($"ls -hal > {outputfile}");  // funcional si tiene permisos 
//var bash = new Bash($"echo  '{content}'> {outputfile}");  //funciona si tiene permisos !
//var bash = new Bash($"echo  '{content}'> {outputfile}", password: "123456");  No funciona
//var bash = new Bash($"echo 123456 | sudo -S touch /etc/netplan/yyy.txt");    // funciona
//var bash = new Bash($"touch /etc/netplan/xxx.txt","123456");  //funciona
//var bash = new Bash($"touch {YAML_FILE}", "123456");  //funciona                    
//var bash = new Bash($"echo 123456 | sudo -S echo '{content}'>{outputfile}");    //  No funciona??