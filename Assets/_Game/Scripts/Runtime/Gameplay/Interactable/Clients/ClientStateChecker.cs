using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.Gameplay.Level;
using Game.Runtime.ServiceLocator;
using Game.Runtime.Services.States;

namespace Game.Runtime.Gameplay.Interactives
{
    public class ClientStateChecker  : IDisposable
    {
        private readonly ClientData _clientData;
        private CancellationTokenSource _checkTokenSource;
        
        public ClientStateChecker(ClientData clientData)
        {
            _clientData = clientData;
        }

        public void HandleStateChanged(ClientState clientState)
        {
            _clientData.Mood = ClientMood.Happy;

            _checkTokenSource = new CancellationTokenSource();
            
            switch (clientState)
            {
                case ClientState.BrowsingMenu:
                {
                    ServiceLocator<WaiterService>.GetService().WaitTimer(3, _checkTokenSource, 
                        () => _clientData.State = ClientState.WaitingToOrder).Forget();
                    break;
                }
                case ClientState.WaitingToOrder:
                {
                    var tableData = ServiceLocator<TableService>.GetService().GetTable(_clientData.TableId);
                    _clientData.View.SetStrategy(new OrderClientInteraction());
                    _clientData.View.ShowHintWarning(true);
                    tableData.Client = _clientData;
                    tableData.Behaviour.SetStrategy(new OrderTableInteraction());
                    break;
                }
                case ClientState.WaitingForFood:
                {
                    _clientData.View.ShowHintWarning(false);
                    _clientData.View.ShowHint(true, _clientData.OrderData.FoodId);

                    break;
                }
                case ClientState.EatingFood:
                {
                    var tableData = ServiceLocator<TableService>.GetService().GetTable(_clientData.TableId);
                    tableData.Food.Behaviour.Interacting = false;
                    
                    _clientData.View.ShowHint(false);
                    ServiceLocator<WaiterService>.GetService().WaitTimer(5, _checkTokenSource, 
                        () => _clientData.State = ClientState.Leaving).Forget();
                    break;
                }
                case ClientState.Leaving:
                {
                    _clientData.View.SetOutline();
                    _clientData.View.SetStrategy(new NotingClientInteraction());
                    
                    if (_clientData.TableId > -1)
                    {
                        var tableData = ServiceLocator<TableService>.GetService().GetTable(_clientData.TableId);
                        if (tableData.Food != null)
                            tableData.Behaviour.SetStrategy(new CleanupTableInteraction());
                        else
                        {
                            tableData.Client = null;
                            tableData.Behaviour.SetStrategy(new EmptyTableInteraction());
                        }
                    }
                    
                    var leavePosition = ServiceLocator<LevelPointsService>.GetService().LeavePoint.position;
                    _clientData.Movable.MoveToPoint(leavePosition, 
                        () => ServiceLocator<ClientsService>.GetService().RemoveClient(_clientData.Id)).Forget();
                    
                    break;
                }
            }
        }
        
        public void Dispose()
        {
            _checkTokenSource?.Dispose();
            _checkTokenSource = null;
        }
    }
}