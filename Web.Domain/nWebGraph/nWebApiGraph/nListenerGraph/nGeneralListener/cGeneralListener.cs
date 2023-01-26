﻿using Bootstrapper.Core.nApplication;
using Data.Domain.nDatabaseService;
using Web.Domain.Controllers;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nCheckLoginCommand;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nLoginCommand;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nLogoutCommand;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nFirstInitCommand;
using Bootstrapper.Core.nHandlers.nLanguageHandler;
using Web.Domain.nWebGraph.nWebApiGraph.nActionGraph.nActions.nLanguageAction;
using Web.Domain.nWebGraph.nWebApiGraph.nActionGraph.nActions.nShowMessageAction;
using Base.Data.nDatabaseService;
using Web.Domain.nWebGraph.nWebApiGraph.nListenerGraph.nLogInOutListener;
using Web.Domain.nWebGraph.nWebApiGraph.nActionGraph.nActions.nPageResultAction;
using Web.Domain.nWebGraph.nWebApiGraph.nListenerGraph.nPermissionListener;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nGetMenuListCommand;
using Data.Domain.nDefaultValueTypes;
using Web.Domain.nWebGraph.nWebApiGraph.nActionGraph.nActions.nMenuResultAction;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nGetPageListCommand;
using Web.Domain.nUtils.nValueTypes;
using Web.Domain.nWebGraph.nWebApiGraph.nActionGraph.nActions.nHotSpotMessageAction;
using Web.Domain.nWebGraph.nSessionManager;
using Web.Domain.nWebGraph.nWebApiGraph.nListenerGraph.nParamListener;
using Web.Domain.nWebGraph.nWebApiGraph.nActionGraph.nActions.nSetGlobalParamListAction;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nGetGlobalParamListCommand;
using Web.Domain.nWebGraph.nWebApiGraph.nCommandGraph.nCommands.nHayriCommand;
using Data.Domain.nDatabaseService.nEntities;

namespace Web.Domain.nWebGraph.nWebApiGraph.nListenerGraph.nGeneralListener
{
     public class cGeneralListener : cBaseListener
        , IFirstInitReceiver
        , IHayriReceiver
    {
        public cGeneralListener(cApp _App, cWebGraph _WebGraph, cDataService _DataService)
               : base(_App, _WebGraph, _DataService)
        {
        }

        public void ReceiveFirstInitData(cListenerEvent _ListenerEvent, IController _Controller, cFirstInitCommandData _ReceivedData)
        {
            WebGraph.ActionGraph.CommandListAction.Action(_Controller);
            WebGraph.ActionGraph.ActionListAction.Action(_Controller);

            if (string.IsNullOrEmpty(_ReceivedData.LanguageCode))
            {
                if (_Controller.ClientSession.IsLogined)
                {
                    _ReceivedData.LanguageCode = _Controller.ClientSession.Language;
                }
                else
                {
                    _ReceivedData.LanguageCode = App.Handlers.LanguageHandler.LanguageNameList[0].Code;
                }
            }
            cLanguageItem __LanguageItem = App.Handlers.LanguageHandler.GetLanguageByCode(_ReceivedData.LanguageCode);
            List<string> __DefinedLanguages = new List<string>();
            foreach (KeyValuePair<string, cLanguageItem> __LanguageItemDictionary in App.Handlers.LanguageHandler.LanguageList)
            {
                __DefinedLanguages.Add(__LanguageItemDictionary.Key);
            }

            if (_Controller.ClientSession.IsLogined)
            {
                cDatabaseContext __DatabaseContext = DataService.GetDatabaseContext();
                __DatabaseContext.Perform(() =>
                {
                    _Controller.ClientSession.User.Language = _ReceivedData.LanguageCode;
                    __DatabaseContext.SaveChanges();
                });
            }

            WebGraph.ActionGraph.LanguageAction.Action(_Controller, new cLanguageProps() { Language = __LanguageItem.LanguageObject, LanguageCode = _ReceivedData.LanguageCode, DefinedLanguages = __DefinedLanguages });

            cMenuResultProps __MenuResultProps = WebGraph.ListenerGraph.GetListenerByType<cPermissionListener>().PrepareMenuResultProps(_Controller, new cGetMenuListCommandData() { MenuTypeCode = MenuTypes.LeftMenu.Code, RootMenuCode = null});
            WebGraph.ActionGraph.MenuResultAction.Action(_Controller, __MenuResultProps);

            cPageResultProps __PageResultProps = WebGraph.ListenerGraph.GetListenerByType<cPermissionListener>().PreparePageResultProps(_Controller, new cGetPageListCommandData());
            WebGraph.ActionGraph.PageResultAction.Action(_Controller, __PageResultProps);

            cSetGlobalParamListProps __GlobalParamListResultProps = WebGraph.ListenerGraph.GetListenerByType<cParamListener>().PrepareGetGlobalParamListProps(_Controller, new cGetGlobalParamListCommandData() );
            WebGraph.ActionGraph.SetGlobalParamListAction.Action(_Controller, __GlobalParamListResultProps);

        }

        public void ReceiveHayriData(cListenerEvent _ListenerEvent, IController _Controller, cHayriCommandData _ReceivedData)
        {
            try
            {
                cDatabaseContext __DatabaseContext = DataService.GetDatabaseContext();

                __DatabaseContext.Perform(() =>
                {
                    cPaymentEntity __PaymentEntity = cPaymentEntity.Add(new cPaymentEntity
                    {
                        Price = _ReceivedData.Price,
                        Name = _ReceivedData.Name
                    });
                    __PaymentEntity.Save();

                });

                WebGraph.ActionGraph.SuccessResultAction.Action(_Controller);
            }
            catch(Exception _Ex)
            {
                cMessageProps __Message = new cMessageProps();

                __Message.Header = "Hate";
                __Message.Message = _Ex.Message;
                __Message.ColorType = EColorTypes.Primary;

                WebGraph.ActionGraph.ShowMessageAction.Action(_Controller, __Message);
            }
            


        }
    }
}
