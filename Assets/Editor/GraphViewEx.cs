using Codice.Client.BaseCommands.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Table;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GraphViewEx : GraphView
{
    private GridBackground _gridBackground = null;

    public GraphViewEx()
    {
        style.flexGrow = 1; // 그래프가 부모를 꽉 채우도록
        style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

        _gridBackground = new GridBackground();
        Insert(0, _gridBackground);          // 0번 인덱스 = 가장 뒤에 깔림
        _gridBackground.StretchToParentSize();

        //Insert(0, new GridBackground() { name = "GridBG" });
        this.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        this.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            Vector2 mousePos = evt.mousePosition;
            evt.menu.AppendAction("Create/Node", _ => CreateNode(mousePos));
        }));

        graphViewChanged = OnGraphViewChanged;
    }

    // 그래프 변경 시 훅(노드/에지 추가·삭제 캐치)
    GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        // 필요하면 여기서 내부 모델 업데이트
        return change;
    }


    public class StateNode : Node
    {
        public string Guid;
        public Port In, Out;

        readonly ScriptableObject _graphAsset; // 그래프 에셋(더티/Undo 위해 보관)
        readonly QuestData _data = null;
        private Action _commit = null;

        public StateNode(string title, Vector2 pos)
        {

            //_data = data;

            Guid = System.Guid.NewGuid().ToString();
            this.title = title;
            SetPosition(new Rect(pos, new Vector2(220, 130)));
            capabilities |= Capabilities.Resizable;

            //var fieldVE = Binder.BindObject(_data, out _commit, out var so);
            //extensionContainer.Add(fieldVE);

            //// 값 변경 시 저장 플래그
            //fieldVE.RegisterCallback<SerializedPropertyChangeEvent>(_ =>
            //{
            //    Undo.RecordObject(_graphAsset, "Edit Node");
            //    _commit();                            // BoxSO → 원본 모델로 값 반영
            //    EditorUtility.SetDirty(_graphAsset);  // 저장 플래그
            //});

            // 입력 포트
            In = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            In.portName = "In";
            inputContainer.Add(In);

            // 출력 포트
            Out = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            Out.portName = "Out";
            outputContainer.Add(Out);

            RefreshExpandedState();
            RefreshPorts();
        }
    }

    public StateNode CreateNode(Vector2 pos, string title = "Node")
    {
        var node = new StateNode(title, pos);
        AddElement(node);
        return node;
    }

    public override List<Port> GetCompatiblePorts(Port start, NodeAdapter _)
    {
        return ports.Where(p =>
            p != start &&
            p.node != start.node &&
            p.direction != start.direction).ToList();
    }
}
