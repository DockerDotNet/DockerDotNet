using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockerDotNet.Core.Services
{
    public class ExecService
    {
        private readonly DockerClient _dockerClient;

        public ExecService(DockerClient dockerClient)
        {
            _dockerClient = dockerClient;
        }

    }
}
