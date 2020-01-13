using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Tests.Puffix.Utilities.Resources
{
    /// <summary>
    /// Conteneur pour la liste des problèmes.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = NAMESPACE)]
    [XmlRoot(Namespace = NAMESPACE, ElementName = ELEMENT_NAME, IsNullable = false)]
    public class IssuesContainer
    {
        /// <summary>
        /// Namespace XML.
        /// </summary>
        public const string NAMESPACE = "urn:sonarqube.org:2019:8.1";

        /// <summary>
        /// Element Name
        /// </summary>
        private const string ELEMENT_NAME = "issues";

        /// <summary>
        /// Liste des problèmes.
        /// </summary>
        [XmlElement("issue")]
        public List<Issue> Issues { get; set; }
    }

    /// <summary>
    /// Problème.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = IssuesContainer.NAMESPACE)]
    public class Issue
    {
        /// <summary>
        /// Identifiant du moteur.
        /// </summary>
        [XmlElement("engineId")]
        public string EngineId { get; set; }

        /// <summary>
        /// Identifiant de la règle.
        /// </summary>
        [XmlElement("ruleId")]
        public string RuleId { get; set; }

        /// <summary>
        /// Gravité.
        /// </summary>
        [XmlElement("severity")]
        public Severity Severity { get; set; }

        /// <summary>
        /// Indique si le champ Severity est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool SeveritySpecified { get; set; }

        /// <summary>
        /// Type de problème.
        /// </summary>
        [XmlElement("type")]
        public IssueType Type { get; set; }

        /// <summary>
        /// Indique si le champ Type est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool TypeSpecified { get; set; }

        /// <summary>
        /// Emplacement principal.
        /// </summary>
        [XmlElement("primaryLocation")]
        public LocationType PrimaryLocation { get; set; }

        /// <summary>
        /// Effort de correction.
        /// </summary>
        [XmlElement("effortMinutes")]
        public int EffortMinutes { get; set; }

        /// <summary>
        /// Indique si le champ EffortMinutes est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool EffortMinutesSpecified { get; set; }

        /// <summary>
        /// secondaryLocations
        /// </summary>
        [XmlElement("secondaryLocations")]
        public List<LocationType> SecondaryLocations { get; set; }
    }

    /// <summary>
    /// Type de problème.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = IssuesContainer.NAMESPACE)]
    public enum IssueType
    {
        /// <summary>
        /// Bug.
        /// </summary>
        [XmlEnum("BUG")]
        Bug,

        /// <summary>
        /// Vulnérabilité.
        /// </summary>
        [XmlEnum("VULNERABILITY")]
        Vulnerability,

        /// <summary>
        /// Code Smell.
        /// </summary>
        [XmlEnum("CODE_SMELL")]
        CodeSmell,
    }

    /// <summary>
    /// Gravité.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = IssuesContainer.NAMESPACE)]
    public enum Severity
    {
        /// <summary>
        /// Bloquant.
        /// </summary>
        [XmlEnum("BLOCKER")]
        Blocker,

        /// <summary>
        /// Critique.
        /// </summary>
        [XmlEnum("CRITICAL")]
        Critical,

        /// <summary>
        /// Majeur.
        /// </summary>
        [XmlEnum("MAJOR")]
        Major,

        /// <summary>
        /// Mineur.
        /// </summary>
        [XmlEnum("MINOR")]
        Minor,

        /// <summary>
        /// Info.
        /// </summary>
        [XmlEnum("INFO")]
        Info
    }

    /// <summary>
    /// Emplacment.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = IssuesContainer.NAMESPACE)]
    public class LocationType
    {
        /// <summary>
        /// Message.
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>
        /// Chemin du fichier.
        /// </summary>
        [XmlElement("filePath")]
        public string FilePath { get; set; }

        /// <summary>
        /// Emplacement dans le fichier.
        /// </summary>
        [XmlElement("textRange")]
        public TextRange TextRange { get; set; }
    }

    /// <summary>
    /// Description de l'emplacement dans le texte.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = IssuesContainer.NAMESPACE)]
    public class TextRange
    {
        /// <summary>
        /// Ligne de début.
        /// </summary>
        [XmlElement("startLine")]
        public int StartLine { get; set; }

        /// <summary>
        /// Indique si le champ StartLine est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool StartLineSpecified { get; set; }

        /// <summary>
        /// Ligne de fin.
        /// </summary>
        [XmlElement("endLine")]
        public int EndLine { get; set; }

        /// <summary>
        /// Indique si le champ EndLine est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool EndLineSpecified { get; set; }

        /// <summary>
        /// Colinne de début.
        /// </summary>
        [XmlElement("startColumn")]
        public int StartColumn { get; set; }

        /// <summary>
        /// Indique si le champ StartColumn est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool StartColumnSpecified { get; set; }

        /// <summary>
        /// Colonne de fin.
        /// </summary>
        [XmlElement("endColumn")]
        public int EndColumn { get; set; }

        /// <summary>
        /// Indique si le champ EndColumn est spécifié.
        /// </summary>
        [XmlIgnore]
        public bool EndColumnSpecified { get; set; }
    }
}