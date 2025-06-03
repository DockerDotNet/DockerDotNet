//using DockerDotNet.Core.Models;
using DockerDotNet.Shared.Models;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DockerDotNet.Shared.Converters;

namespace DockerDotNet.Shared.Extensions
{
    public static class IServiceCollectionExtension
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions();

        public static void AddJsonSerializerOptions(this IServiceCollection serviceCollection)
        {
            _jsonOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
            _jsonOptions.Converters.Add(new DateTimeJsonConverter());
            _jsonOptions.Converters.Add(new DateTimeNullableJsonConverter());
            _jsonOptions.Converters.Add(new DateOnlyJsonConverter());
            _jsonOptions.Converters.Add(new DateOnlyNullableJsonConverter());
            _jsonOptions.Converters.Add(new BuildCache.TypeEnumJsonConverter());
            _jsonOptions.Converters.Add(new BuildCache.TypeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ChangeTypeJsonConverter());
            _jsonOptions.Converters.Add(new ChangeTypeNullableJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumePublishStatusInner.StateEnumJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumePublishStatusInner.StateEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumeSpecAccessMode.ScopeEnumJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumeSpecAccessMode.ScopeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumeSpecAccessMode.SharingEnumJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumeSpecAccessMode.SharingEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumeSpecAccessMode.AvailabilityEnumJsonConverter());
            _jsonOptions.Converters.Add(new ClusterVolumeSpecAccessMode.AvailabilityEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ContainerState.StatusEnumJsonConverter());
            _jsonOptions.Converters.Add(new ContainerState.StatusEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ContainerSummary.StateEnumJsonConverter());
            _jsonOptions.Converters.Add(new ContainerSummary.StateEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new EndpointPortConfig.ProtocolEnumJsonConverter());
            _jsonOptions.Converters.Add(new EndpointPortConfig.ProtocolEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new EndpointPortConfig.PublishModeEnumJsonConverter());
            _jsonOptions.Converters.Add(new EndpointPortConfig.PublishModeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new EndpointSpec.ModeEnumJsonConverter());
            _jsonOptions.Converters.Add(new EndpointSpec.ModeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new EventMessage.TypeEnumJsonConverter());
            _jsonOptions.Converters.Add(new EventMessage.TypeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new EventMessage.ScopeEnumJsonConverter());
            _jsonOptions.Converters.Add(new EventMessage.ScopeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new Health.StatusEnumJsonConverter());
            _jsonOptions.Converters.Add(new Health.StatusEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new HostConfig.CgroupnsModeEnumJsonConverter());
            _jsonOptions.Converters.Add(new HostConfig.CgroupnsModeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new HostConfig.IsolationEnumJsonConverter());
            _jsonOptions.Converters.Add(new HostConfig.IsolationEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new HostConfigAllOfLogConfig.TypeEnumJsonConverter());
            _jsonOptions.Converters.Add(new HostConfigAllOfLogConfig.TypeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ImageManifestSummary.KindEnumJsonConverter());
            _jsonOptions.Converters.Add(new ImageManifestSummary.KindEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new LocalNodeStateJsonConverter());
            _jsonOptions.Converters.Add(new LocalNodeStateNullableJsonConverter());
            _jsonOptions.Converters.Add(new Mount.TypeEnumJsonConverter());
            _jsonOptions.Converters.Add(new Mount.TypeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new MountBindOptions.PropagationEnumJsonConverter());
            _jsonOptions.Converters.Add(new MountBindOptions.PropagationEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new MountPoint.TypeEnumJsonConverter());
            _jsonOptions.Converters.Add(new MountPoint.TypeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new NodeSpec.RoleEnumJsonConverter());
            _jsonOptions.Converters.Add(new NodeSpec.RoleEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new NodeSpec.AvailabilityEnumJsonConverter());
            _jsonOptions.Converters.Add(new NodeSpec.AvailabilityEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new NodeStateJsonConverter());
            _jsonOptions.Converters.Add(new NodeStateNullableJsonConverter());
            _jsonOptions.Converters.Add(new PluginConfigInterface.ProtocolSchemeEnumJsonConverter());
            _jsonOptions.Converters.Add(new PluginConfigInterface.ProtocolSchemeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new Port.TypeEnumJsonConverter());
            _jsonOptions.Converters.Add(new Port.TypeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ReachabilityJsonConverter());
            _jsonOptions.Converters.Add(new ReachabilityNullableJsonConverter());
            _jsonOptions.Converters.Add(new RestartPolicy.NameEnumJsonConverter());
            _jsonOptions.Converters.Add(new RestartPolicy.NameEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecRollbackConfig.FailureActionEnumJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecRollbackConfig.FailureActionEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecRollbackConfig.OrderEnumJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecRollbackConfig.OrderEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecUpdateConfig.FailureActionEnumJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecUpdateConfig.FailureActionEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecUpdateConfig.OrderEnumJsonConverter());
            _jsonOptions.Converters.Add(new ServiceSpecUpdateConfig.OrderEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new ServiceUpdateStatus.StateEnumJsonConverter());
            _jsonOptions.Converters.Add(new ServiceUpdateStatus.StateEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new SwarmSpecCAConfigExternalCAsInner.ProtocolEnumJsonConverter());
            _jsonOptions.Converters.Add(new SwarmSpecCAConfigExternalCAsInner.ProtocolEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new SystemInfo.CgroupDriverEnumJsonConverter());
            _jsonOptions.Converters.Add(new SystemInfo.CgroupDriverEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new SystemInfo.CgroupVersionEnumJsonConverter());
            _jsonOptions.Converters.Add(new SystemInfo.CgroupVersionEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new SystemInfo.IsolationEnumJsonConverter());
            _jsonOptions.Converters.Add(new SystemInfo.IsolationEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecContainerSpec.IsolationEnumJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecContainerSpec.IsolationEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecContainerSpecPrivilegesAppArmor.ModeEnumJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecContainerSpecPrivilegesAppArmor.ModeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecContainerSpecPrivilegesSeccomp.ModeEnumJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecContainerSpecPrivilegesSeccomp.ModeEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecRestartPolicy.ConditionEnumJsonConverter());
            _jsonOptions.Converters.Add(new TaskSpecRestartPolicy.ConditionEnumNullableJsonConverter());
            _jsonOptions.Converters.Add(new TaskStateJsonConverter());
            _jsonOptions.Converters.Add(new TaskStateNullableJsonConverter());
            _jsonOptions.Converters.Add(new Volume.ScopeEnumJsonConverter());
            _jsonOptions.Converters.Add(new Volume.ScopeEnumNullableJsonConverter());

            serviceCollection.AddSingleton(_jsonOptions);
        }
    }
}
