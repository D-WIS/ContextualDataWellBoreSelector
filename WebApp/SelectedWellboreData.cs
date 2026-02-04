using DWIS.RigOS.Common.Model;
using DWIS.Vocabulary.Schemas;
using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using DWIS.ContextualData.WellBoreSelector.ModelShared;

namespace WebApp
{
    public class WellBoreProperty : SemanticInfo
    {
        public WellBore? WellBore { get; set; } = null;
    }
    public class SelectedWellboreData
    {
        [AccessToVariable(CommonProperty.VariableAccessType.Assignable)]
        [Mandatory(CommonProperty.MandatoryType.General)]
        [SemanticStringVariable("SelectedWellboreSignal")]
        [SemanticFact("SelectedWellboreSignal", Nouns.Enum.DynamicDrillingSignal)]
        [SemanticFact("SelectedWellboreSignal#01", Nouns.Enum.DrillingDataPoint)]
        [SemanticFact("SelectedWellboreSignal#01", Nouns.Enum.WellBoreData)] // to be replaced by Nouns.Enum.WellBore
        [SemanticFact("SelectedWellboreSignal#01", Nouns.Enum.OperationalPlan)] // to be removed when replaced by Nouns.Enum.WellBore
        [SemanticFact("SelectedWellboreSignal#01", Nouns.Enum.JSonDataType)]
        [SemanticFact("SelectedWellboreSignal#01", Verbs.Enum.HasDynamicValue, "SelectedWellboreSignal")]
        [SemanticFact("Current#01", Nouns.Enum.Current)]
        [SemanticFact("SelectedWellboreSignal#01", Verbs.Enum.IsCharacterizedBy, "Current#01")]
        [SemanticFact("Advisor#01", Nouns.Enum.Advisor)]
        [SemanticFact("SelectedWellboreSignal#01", Verbs.Enum.IsProvidedTo, "Advisor#01")]
        [SemanticFact("DataProvider#01", Nouns.Enum.DataProvider)]
        [SemanticFact("SelectedWellboreSignal#01", Verbs.Enum.IsProvidedTo, "DataProvider#01")]
        [SemanticFact("contextualDataBuilder#01", Nouns.Enum.DWISContextualDataBuilder)]
        [SemanticFact("SelectedWellboreSignal#01", Verbs.Enum.IsProvidedBy, "contextualDataBuilder#01")]
        public WellBoreProperty WellBore { get; set; } = new WellBoreProperty();
    }
}
