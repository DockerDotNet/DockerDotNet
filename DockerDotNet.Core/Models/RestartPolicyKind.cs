﻿using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DockerDotNet.Core.Models
{
    public enum RestartPolicyKind
    {
        [EnumMember(Value = "")]
        Undefined,

        [EnumMember(Value = "no")]
        No,

        [EnumMember(Value = "always")]
        Always,

        [EnumMember(Value = "on-failure")]
        OnFailure,

        [EnumMember(Value = "unless-stopped")]
        UnlessStopped
    }
}
