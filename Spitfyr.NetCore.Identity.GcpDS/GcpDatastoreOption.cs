﻿namespace Spitfyr.NetCore.Identity.GcpDS
{
    public class GcpDatastoreOption
    {
        public string CredentialsFilePath { get; set; }
        public string ProjectId { get; set; }
        public string Namespace { get; set; }
        
        public Option User { get; set; } = new Option();
        public Option Role { get; set; } = new Option();
    }

    public class Option
    {
        public string Kind { get; set; } = "Users";
        public bool ManageIndicies { get; set; } = true;
    }
}