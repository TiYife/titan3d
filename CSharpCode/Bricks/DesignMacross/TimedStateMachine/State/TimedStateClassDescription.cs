﻿using EngineNS.Bricks.CodeBuilder;
using EngineNS.Bricks.StateMachine.TimedSM;
using EngineNS.DesignMacross.Description;
using EngineNS.DesignMacross.Graph;
using EngineNS.DesignMacross.Outline;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EngineNS.DesignMacross.TimedStateMachine
{
    [StateMachineContextMenu("State", "StateMachine\\State", UMacross.MacrossEditorKeyword)]
    [OutlineElement(typeof(TtOutlineElement_TimedState))]
    [GraphElement(typeof(TtGraphElement_TimedState))]
    public class TtTimedStateClassDescription : IDesignableVariableDescription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "TimedState";
        public string VariableName { get => TtDescriptionUtil.VariableNamePrefix + Name; }
        public string ClassName { get => TtDescriptionUtil.ClassNamePrefix + Name; }

        public EVisisMode VisitMode { get; set; } = EVisisMode.Public;
        public UCommentStatement Comment { get; set; }
        public UTypeReference VariableType { get => new UTypeReference(ClassName); set { } } 
        public UExpressionBase InitValue { get; set; }
        public UNamespaceDeclaration Namespace { get; set; }
        public bool IsStruct { get; set; }
        public List<string> SupperClassNames { get; set; } = new List<string>();
        public ObservableCollection<IVariableDescription> Variables { get; set; } = new ObservableCollection<IVariableDescription>();
        public ObservableCollection<IMethodDescription> Methods { get; set; } = new ObservableCollection<IMethodDescription>();

        public ObservableCollection<TtTimedStateTransitionClassDescription> Transitions = new ObservableCollection<TtTimedStateTransitionClassDescription>();
        public List<UClassDeclaration> BuildClassDeclarations()
        {
            List<UClassDeclaration> classDeclarationsBuilded = new();
            UClassDeclaration thisClassDeclaration = new();
            foreach (var transition in Transitions)
            {
                classDeclarationsBuilded.AddRange(transition.BuildClassDeclarations());
                thisClassDeclaration.Properties.Add(transition.BuildVariableDeclaration());
            }
            classDeclarationsBuilded.Add(thisClassDeclaration);
            return classDeclarationsBuilded;
        }

        public UVariableDeclaration BuildVariableDeclaration()
        {
            return TtDescriptionUtil.BuildDefaultPartForVariableDeclaration(this);
        }
    }
}
