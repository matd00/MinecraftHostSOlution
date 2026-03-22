using System.Runtime.InteropServices;
using Docker.DotNet;
using Docker.DotNet.Models;
using ServerHost.Domain.Interfaces;

namespace ServerHost.Infrastructure.Docker;

public class DockerService : IDockerService
{
    private readonly DockerClient _client;
    private const string ImageName = "itzg/minecraft-server";

    public DockerService()
    {
        string endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "npipe://./pipe/docker_engine"
            : "unix:///var/run/docker.sock";

        _client = new DockerClientConfiguration(new Uri(endpoint)).CreateClient();
    }

    public async Task CreateContainerAsync(string name, int port)
    {
        // Ensure image exists
        await _client.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = ImageName, Tag = "latest" },
            null,
            new Progress<JSONMessage>());

        var config = new Config
        {
            Image = ImageName,
            Env = new List<string> { "EULA=TRUE", "TYPE=VANILLA" },
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { "25565/tcp", new EmptyStruct() }
            }
        };

        var hostConfig = new HostConfig
        {
            PortBindings = new Dictionary<string, IList<PortBinding>>
            {
                { "25565/tcp", new List<PortBinding> { new PortBinding { HostPort = port.ToString() } } }
            },
            Mounts = new List<Mount>
            {
                new Mount { Type = "volume", Source = $"mc-data-{name}", Target = "/data" }
            }
        };

        await _client.Containers.CreateContainerAsync(new CreateContainerParameters(config)
        {
            Name = name,
            HostConfig = hostConfig
        });
    }

    public async Task StartContainerAsync(string name)
    {
        await _client.Containers.StartContainerAsync(name, null);
    }

    public async Task StopContainerAsync(string name)
    {
        await _client.Containers.StopContainerAsync(name, new ContainerStopParameters());
    }

    public async Task RemoveContainerAsync(string name)
    {
        await _client.Containers.RemoveContainerAsync(name, new ContainerRemoveParameters { Force = true });
    }

    public async Task<bool> IsRunningAsync(string name)
    {
        try
        {
            var container = await _client.Containers.InspectContainerAsync(name);
            return container.State.Running;
        }
        catch (DockerContainerNotFoundException)
        {
            return false;
        }
    }
}
