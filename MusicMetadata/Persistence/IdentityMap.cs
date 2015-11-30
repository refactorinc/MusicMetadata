using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicMetadata.Persistence;

namespace MusicMetadata.Persistence
{
    static class IdentityMap
    {
        static Hashtable _metadataDtoMap = new Hashtable();

        public static MetadataDto Get(string id)
        {
            return (MetadataDto) _metadataDtoMap[id];
        }

        public static void Put(MetadataDto dto)
        {
            _metadataDtoMap[dto.Id] = dto;
        }
    }
}
