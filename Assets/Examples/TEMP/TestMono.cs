// using System;
// using Cysharp.Threading.Tasks;
// using SoyoFramework.Framework.Runtime.LogKit;
// using SoyoFramework.Framework.Runtime.ProcedureKit;
// using SoyoFramework.Framework.Runtime.ProcedureKit.GeneratedClasses;
// using UnityEngine;
//
// namespace Examples.TEMP
// {
//     public class TestMono : MonoBehaviour
//     {
//         private IProcedureManager _procedureManager;
//
//         private void Awake()
//         {
//             _procedureManager = new ProcedureManager();
//             _procedureManager.RegisterProcedure(ProcedureId.ExampleProcedureA, ProcedureChangeStage.EnterNormal,
//                 _ => { "Enter Normal ProcedureA".LogInfo(); });
//
//             _procedureManager.RegisterProcedure(ProcedureId.ExampleProcedureA, ProcedureChangeStage.LeaveNormal, _ =>
//             {
//                 "Leave Normal ProcedureA".LogInfo();
//                 _procedureManager.AddAwait(UniTask.WaitForSeconds(1f));
//             });
//
//             _procedureManager.RegisterProcedure(ProcedureId.ExampleProcedureA, ProcedureChangeStage.LeaveLate,
//                 _ => { "Leave Late ProcedureA".LogInfo(); });
//
//             _procedureManager.RegisterProcedure(ProcedureId.ExampleProcedureB, ProcedureChangeStage.EnterNormal,
//                 _ => { "Enter Normal ProcedureB".LogInfo(); });
//         }
//
//         private void OnGUI()
//         {
//             if (GUILayout.Button("Go to Procedure A"))
//             {
//                 _procedureManager.ChangeProcedure(ProcedureId.ExampleProcedureA);
//             }
//
//             if (GUILayout.Button("Go to Procedure B"))
//             {
//                 _procedureManager.ChangeProcedure(ProcedureId.ExampleProcedureB);
//             }
//         }
//     }
// }

