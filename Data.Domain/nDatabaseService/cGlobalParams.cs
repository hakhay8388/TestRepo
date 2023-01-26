﻿using Bootstrapper.Core.nApplication;
using Bootstrapper.Core.nCore;
using Data.Domain.nDatabaseService.nSystemEntities;
using Data.Domain.nDataService.nDataManagers;
using Data.Domain.nDefaultValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Domain.nDatabaseService
{
    public class cGlobalParams : cCoreObject
    {
        public string TestParm1 { get; set; }

        public bool FrontEndDebugMessage { get; set; }

        public bool BackendDebugMessageShowToUser { get; set; }


        public List<object> GlobalParamList = null;
        public List<object> PublicParamList = null;
        public List<object> PrivateParamList = null;

        public cGlobalParams(cApp _App)
            :base(_App)
        {

        }
        public void LoadGlobalParams()
        {
            GlobalParamList = new List<object>();
            PublicParamList = new List<object>();
            PrivateParamList = new List<object>();

            cGlobalParamsDataManager __ParamsDataManager = cApp.App.Factories.ObjectFactory.ResolveInstance<cGlobalParamsDataManager>();
            for (int i = 0; i < DefaultGlobalParamsIDs.TypeList.Count; i++)
            {
                cGlobalParamEntity __GlobalParamEntity = __ParamsDataManager.GetParamByCode(DefaultGlobalParamsIDs.TypeList[i].Code);
                Type __Type = Type.GetType(__GlobalParamEntity.TypeFullName);
                try
                {
                    object __TempValue = Convert.ChangeType(__GlobalParamEntity.Value, __Type);
                    var __ThisType = this.GetType();
                    __ThisType.SetPropertyValue(this, __GlobalParamEntity.Code, __TempValue);

                    GlobalParamList.Add(new
                    {
                        ParamName = DefaultGlobalParamsIDs.TypeList[i].Code,
                        ParamValue = __TempValue
                    });
                    if (DefaultGlobalParamsIDs.TypeList[i].IsPrivate)
                    {
                        PrivateParamList.Add(new
                        {
                            ParamName = DefaultGlobalParamsIDs.TypeList[i].Code,
                            ParamValue = __TempValue
                        });
                    }
                    else
                    {
                        PublicParamList.Add(new
                        {
                            ParamName = DefaultGlobalParamsIDs.TypeList[i].Code,
                            ParamValue = __TempValue
                        });
                    }
                }
                catch (Exception _Ex)
                {
                    App.Loggers.CoreLogger.LogError(_Ex);
                    throw _Ex;
                }
            }

        }
    }
}
