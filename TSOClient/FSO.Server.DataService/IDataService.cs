﻿using FSO.Common.Security;
using FSO.Common.Serialization.Primitives;
using FSO.Server.DataService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSO.Common.DataService
{
    public interface IDataService
    {
        Task<T> Get<T>(object key);
        Task<T[]> GetMany<T>(object[] keys);

        Task<object> Get(Type type, object key);
        Task<object> Get(uint type, object key);
        Task<object> Get(MaskedStruct type, object key);


        List<cTSOTopicUpdateMessage> SerializeUpdate(MaskedStruct mask, object value, uint id);
        Task<cTSOTopicUpdateMessage> SerializePath(params uint[] dotPath);

        void ApplyUpdate(cTSOTopicUpdateMessage update, ISecurityContext context);
        
    }
}
