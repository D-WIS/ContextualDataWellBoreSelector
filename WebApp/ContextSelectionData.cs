using DWIS.RigOS.Common.Model;
using DWIS.Vocabulary.Schemas;
using OSDC.DotnetLibraries.Drilling.DrillingProperties;
namespace WebApp;
public class ContextSelectionData
{
        [AccessToVariable(CommonProperty.VariableAccessType.Assignable)]
        [Mandatory(CommonProperty.MandatoryType.General)]
        [SemanticStringVariable("SelectedWellBoreArchitectureUUID")]
        [SemanticFact("SelectedWellBoreArchitectureUUID", Nouns.Enum.DynamicDrillingSignal)]
        [SemanticFact("SelectedWellBoreArchitectureUUID#01", Nouns.Enum.DrillingDataPoint)]
        [SemanticFact("SelectedWellBoreArchitectureUUID#01", Nouns.Enum.ConfigurationData)]
        [SemanticFact("SelectedWellBoreArchitectureUUID#01", Nouns.Enum.WellboreArchitectureDescription)]
        [SemanticFact("SelectedWellBoreArchitectureUUID#01", Nouns.Enum.StringDataType)]
        [SemanticFact("SelectedWellBoreArchitectureUUID#01", Verbs.Enum.HasDynamicValue, "SelectedWellBoreArchitectureUUID")]
        [SemanticFact("Current#01", Nouns.Enum.Current)]
        [SemanticFact("SelectedWellBoreArchitectureUUID#01", Verbs.Enum.IsCharacterizedBy, "Current#01")]
        public SemanticInfo WellBoreArchitecture { get; set; } = new();
        [AccessToVariable(CommonProperty.VariableAccessType.Assignable)]
        [Mandatory(CommonProperty.MandatoryType.General)]
        [SemanticStringVariable("SelectedTrajectoryUUID")]
        [SemanticFact("SelectedTrajectoryUUID", Nouns.Enum.DynamicDrillingSignal)]
        [SemanticFact("SelectedTrajectoryUUID#01", Nouns.Enum.DrillingDataPoint)]
        [SemanticFact("SelectedTrajectoryUUID#01", Nouns.Enum.ConfigurationData)]
        [SemanticFact("SelectedTrajectoryUUID#01", Nouns.Enum.TrajectoryDescription)]
        [SemanticFact("SelectedTrajectoryUUID#01", Nouns.Enum.StringDataType)]
        [SemanticFact("SelectedTrajectoryUUID#01", Verbs.Enum.HasDynamicValue, "SelectedTrajectoryUUID")]
        [SemanticFact("Current#01", Nouns.Enum.Current)]
        [SemanticFact("SelectedTrajectoryUUID#01", Verbs.Enum.IsCharacterizedBy, "Current#01")]
        public SemanticInfo Trajectory { get; set; } = new();
        [AccessToVariable(CommonProperty.VariableAccessType.Assignable)]
        [Mandatory(CommonProperty.MandatoryType.General)]
        [SemanticStringVariable("SelectedDrillStringUUID")]
        [SemanticFact("SelectedDrillStringUUID", Nouns.Enum.DynamicDrillingSignal)]
        [SemanticFact("SelectedDrillStringUUID#01", Nouns.Enum.DrillingDataPoint)]
        [SemanticFact("SelectedDrillStringUUID#01", Nouns.Enum.ConfigurationData)]
        [SemanticFact("SelectedDrillStringUUID#01", Nouns.Enum.DrillStringDescription)]
        [SemanticFact("SelectedDrillStringUUID#01", Nouns.Enum.StringDataType)]
        [SemanticFact("SelectedDrillStringUUID#01", Verbs.Enum.HasDynamicValue, "SelectedDrillStringUUID")]
        [SemanticFact("Current#01", Nouns.Enum.Current)]
        [SemanticFact("SelectedDrillStringUUID#01", Verbs.Enum.IsCharacterizedBy, "Current#01")]
        public SemanticInfo DrillString { get; set; } = new();
}
