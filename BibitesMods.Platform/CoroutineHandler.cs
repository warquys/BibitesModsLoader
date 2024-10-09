using Neuron.Core;
using Neuron.Core.Scheduling;
using UnityEngine;

namespace BibitesMods.Platform;

public class CoroutineHandler : MonoBehaviour
{
    #region Properties & Variables
    public Action Ticker { get; set; }
    #endregion

	#region Unity
    void Awake()
    {
        DontDestroyOnLoad(base.gameObject);
    }

    void Start()
	{
        Ticker = Globals.Get<ActionCoroutineReactor>().GetTickAction();
    }

    void Update()
	{
        Ticker.Invoke();
    }
	#endregion
}
