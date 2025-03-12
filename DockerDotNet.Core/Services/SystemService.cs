using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Services
{
    public class SystemService
    {
        private readonly DockerClient _dockerClient;

        public SystemService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }



    }
}
