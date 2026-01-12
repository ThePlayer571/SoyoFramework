using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace SoyoFramework.OptionalKits.UIKit.Runtime
{
    public class UIRoot : MonoBehaviour
    {
        [field: SerializeField] [NotNull] public Camera UICamera { get; private set; }
        [field: SerializeField] [NotNull] public Canvas Canvas { get; private set; }
        [SerializeField] internal UIManager UIManager;
    }
}