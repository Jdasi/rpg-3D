using UnityEngine;
using UnityEngine.UI;

public class EffectPopup : MonoBehaviour
{
	[SerializeField] Text _text;
	[SerializeField] float _duration;

	private float _timer;

	public void Init(string text, Color color)
	{
		_text.text = text;
		_text.color = color;
	}

    private void Update()
    {
        _timer += Time.deltaTime;

		if (_timer > _duration)
		{
			Destroy(this.gameObject);
		}
    }
}
