#if XNODE_SUPPORT

using System;
using System.Collections.Generic;
using System.Linq;
using SoyoFramework.OptionalKits.ProcedureKit.Editor;
using SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor.Nodes;
using UnityEditor;
using UnityEngine;
using XNode;

namespace SoyoFramework.OptionalKits.ProcedureKit.xNodeEditor
{
    [CreateAssetMenu(menuName = "SoyoFramework/ProcedureKit/ProcedureChangeRuleGraph",
        fileName = "ProcedureChangeRuleGraph", order = 0)]
    public class ProcedureChangeRuleGraph : NodeGraph
    {
        [SerializeField] private ProcedureKitConfigSO _attachedConfig;

        public ProcedureKitConfigSO AttachedConfig
        {
            get => _attachedConfig;
            set => _attachedConfig = value;
        }

        public void ReadConfig()
        {
            if (AttachedConfig == null)
            {
                Debug.LogError("AttachedConfig is null, cannot read config");
                return;
            }

            Undo.RecordObject(this, "Read Procedure Config");

            // 创建配置条目字典，便于快速查找
            var entryDict = AttachedConfig.Procedures.ToDictionary(entry => entry.EnumValue, entry => entry);

            // === 第一步：分析现有节点 ===
            var toRemove = new List<Node>();
            var existingEnumValues = new HashSet<int>();
            Node entranceNode = null;

            foreach (var node in nodes)
            {
                switch (node)
                {
                    case EntranceNode entrance:
                        entranceNode = entrance;
                        break;
                    case ProcedureNode procedureNode:
                    {
                        // 检查节点是否在配置中存在
                        if (!entryDict.ContainsKey(procedureNode.EnumValue))
                        {
                            // 配置中不存在的节点需要移除
                            toRemove.Add(node);
                        }
                        else
                        {
                            // 记录已存在的节点，并更新名称
                            existingEnumValues.Add(procedureNode.EnumValue);
                            procedureNode.name = entryDict[procedureNode.EnumValue].Name;
                        }

                        break;
                    }
                }
            }

            // === 第二步：移除不合法的节点 ===
            foreach (var node in toRemove)
            {
                AssetDatabase.RemoveObjectFromAsset(node);
                RemoveNode(node);
            }

            var nodeX = 0;
            // === 第三步：确保 Entrance 节点存在 ===
            if (entranceNode == null)
            {
                entranceNode = AddNode<EntranceNode>();
                entranceNode.name = "Entrance";
                entranceNode.position = new Vector2(nodeX, 0);
                nodeX += 200;
                AssetDatabase.AddObjectToAsset(entranceNode, this);
            }

            // === 第四步：添加配置中存在但图中缺失的节点 ===
            foreach (var entry in entryDict.Values.OrderBy(e => e.EnumValue))
            {
                // Entrance 节点已经处理过了
                if (entry.EnumValue == 0) continue;

                if (!existingEnumValues.Contains(entry.EnumValue))
                {
                    var procedureNode = AddNode<ProcedureNode>();
                    procedureNode.name = entry.Name;
                    procedureNode.EnumValue = entry.EnumValue;
                    procedureNode.position = new Vector2(nodeX, 0);
                    nodeX += 200;
                    AssetDatabase.AddObjectToAsset(procedureNode, this);
                }
            }

            // === 第五步：根据配置重建节点连接 ===
            // 构建节点字典，便于查找
            var nodeDict = nodes.OfType<ProcedureNode>()
                .ToDictionary(node => node.EnumValue, node => (Node)node);

            // 为每个 Procedure 节点建立连接
            foreach (var procedureNode in nodes.OfType<ProcedureNode>())
            {
                var entry = entryDict[procedureNode.EnumValue];

                // 获取输入端口并清除旧连接
                var previousPort = procedureNode.GetInputPort("previous");
                if (previousPort == null)
                {
                    Debug.LogWarning($"ProcedureNode '{procedureNode.name}' does not have 'previous' port");
                    continue;
                }

                previousPort.ClearConnections();

                // 根据配置建立新连接
                foreach (var previousEnumValue in entry.AllowedPreviousEnumValues)
                {
                    Node previousNode = null;

                    if (previousEnumValue == 0)
                    {
                        // 连接到 Entrance
                        previousNode = entranceNode;
                    }
                    else if (nodeDict.TryGetValue(previousEnumValue, out var foundNode))
                    {
                        // 连接到其他 Procedure
                        previousNode = foundNode;
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Cannot find previous node with EnumValue {previousEnumValue} for '{procedureNode.name}'");
                        continue;
                    }

                    if (previousNode != null)
                    {
                        var nextPort = previousNode.GetOutputPort("next");
                        if (nextPort != null)
                        {
                            nextPort.Connect(previousPort);
                        }
                        else
                        {
                            Debug.LogWarning($"Previous node '{previousNode.name}' does not have 'next' port");
                        }
                    }
                }
            }

            // === 第六步：标记为脏数据 ===
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            Debug.Log($"Config read successfully. Total nodes: {nodes.Count} (Procedures: {nodeDict.Count})");
        }

        public void WriteConfig()
        {
            if (AttachedConfig == null)
            {
                Debug.LogError("AttachedConfig is null, cannot write config");
                return;
            }

            Undo.RecordObject(AttachedConfig, "Write Procedure Config");

            // 创建新的 Procedures 列表
            var newProcedures = new List<ProcedureKitConfigSO.ProcedureEntry>();

            // 首先添加 Entrance (EnumValue = 0)
            var entranceEntry = new ProcedureKitConfigSO.ProcedureEntry
            {
                Name = "Entrance",
                EnumValue = 0,
                TagEnumValues = new List<int>(),
                AllowedPreviousEnumValues = new List<int>() // Entrance 没有前置节点
            };
            newProcedures.Add(entranceEntry);

            // 收集所有 ProcedureNode 的信息
            var procedureNodes = nodes.OfType<ProcedureNode>().OrderBy(n => n.EnumValue).ToList();

            // 为了保留原有的 TagEnumValues，创建一个字典
            var oldEntriesDict = AttachedConfig.Procedures.ToDictionary(
                entry => entry.EnumValue,
                entry => entry
            );

            foreach (var procedureNode in procedureNodes)
            {
                var entry = new ProcedureKitConfigSO.ProcedureEntry
                {
                    Name = procedureNode.name,
                    EnumValue = procedureNode.EnumValue,
                    TagEnumValues = new List<int>(),
                    AllowedPreviousEnumValues = new List<int>()
                };

                // 保留原有的 TagEnumValues（如果存在）
                if (oldEntriesDict.TryGetValue(procedureNode.EnumValue, out var oldEntry))
                {
                    entry.TagEnumValues = new List<int>(oldEntry.TagEnumValues);
                }

                // 如果 ProcedureNode 有 TagEnumValues 属性，优先使用它
                // entry.TagEnumValues = new List<int>(procedureNode.TagEnumValues);

                // 获取输入端口的连接，确定 AllowedPreviousEnumValues
                var previousPort = procedureNode.GetInputPort("previous");
                if (previousPort != null && previousPort.ConnectionCount > 0)
                {
                    foreach (var connection in previousPort.GetConnections())
                    {
                        var connectedNode = connection.node;

                        switch (connectedNode)
                        {
                            case EntranceNode:
                                // 连接到 Entrance，添加 EnumValue = 0
                                if (!entry.AllowedPreviousEnumValues.Contains(0))
                                {
                                    entry.AllowedPreviousEnumValues.Add(0);
                                }

                                break;
                            case ProcedureNode previousProcedure:
                                // 连接到其他 Procedure
                                if (!entry.AllowedPreviousEnumValues.Contains(previousProcedure.EnumValue))
                                {
                                    entry.AllowedPreviousEnumValues.Add(previousProcedure.EnumValue);
                                }

                                break;
                        }
                    }
                }

                // 排序以保持一致性
                entry.AllowedPreviousEnumValues.Sort();

                newProcedures.Add(entry);
            }

            // 更新配置
            AttachedConfig.Procedures = newProcedures;

            // 标记为脏数据
            EditorUtility.SetDirty(AttachedConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Config written successfully. Total procedures: {newProcedures.Count}");
        }
    }
}

#endif