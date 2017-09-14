// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Microsoft.Azure.Commands.Dns.Models;
using Microsoft.Azure.Management.Dns.Models;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using ProjectResources = Microsoft.Azure.Commands.Dns.Properties.Resources;

namespace Microsoft.Azure.Commands.Dns
{
    /// <summary>
    /// Creates a new zone.
    /// </summary>
    [Cmdlet(VerbsCommon.New, "AzureRmDnsZone", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium), OutputType(typeof(DnsZone))]
    public class NewAzureDnsZone : DnsBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The full name of the zone (without a terminating dot).")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "The resource group in which to create the zone.")]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }
        
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The type of the zone, public or private. This property cannot be changed for a zone")]
        [ValidateNotNullOrEmpty]
        public ZoneType? ZoneType  { get; set; }
        
        [Alias("Tags")]
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "A hash table which represents resource tags.")]
        public Hashtable Tag { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The list of virtual networks able to resolve records in this DNS zone, only available for private zones.", ParameterSetName = "Fields")]
        [ValidateNotNull]
        public List<string> ResolutionVirtualNetworkIds { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "The list of virtual networks that will register VM hostnames records in this DNS zone, only available for private zones.", ParameterSetName = "Fields")]
        [ValidateNotNull]
        public List<string> RegistrationVirtualNetworkIds { get; set; }


        public override void ExecuteCmdlet()
        {
            WriteWarning("The output object type of this cmdlet will be modified in a future release.");

            if (this.Name.EndsWith("."))
            {
                this.Name = this.Name.TrimEnd('.');
                this.WriteWarning(string.Format("Modifying zone name to remove terminating '.'.  Zone name used is \"{0}\".", this.Name));
            }

            ConfirmAction(
                ProjectResources.Progress_CreatingNewZone,
                this.Name,
            () =>
            {
                DnsZone result = this.DnsClient.CreateDnsZone(
                    this.Name,
                    this.ResourceGroupName,
                    this.Tag,
                    this.ZoneType != null ? this.ZoneType.Value : Management.Dns.Models.ZoneType.Public,
                    this.RegistrationVirtualNetworkIds,
                    this.ResolutionVirtualNetworkIds);
                this.WriteVerbose(ProjectResources.Success);
                this.WriteVerbose(string.Format(ProjectResources.Success_NewZone, this.Name, this.ResourceGroupName));
                this.WriteObject(result);
            });
        }
    }
}
