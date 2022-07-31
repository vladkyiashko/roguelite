using System.Collections;
using UnityEngine;

public class MobHealth : AbstractHealth
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    private Shader ShaderGUIText;
    private Shader ShaderDefault;
    private Coroutine SpriteColorCor;
    private const float WhiteSpriteColorTime = 0.3f;
    private WaitForSeconds WhiteSpriteColorWait;

    private void Awake()
    {
        WhiteSpriteColorWait = new(WhiteSpriteColorTime);
        ShaderDefault = SpriteRenderer.material.shader;
        ShaderGUIText = Shader.Find("GUI/Text Shader");
    }

    private void OnEnable()
    {
        SpriteRenderer.material.shader = ShaderDefault;
    }

    protected override void CommonDamage(float damage)
    {
        base.CommonDamage(damage);

        SpriteColorCor = StartCoroutine(ChangeSpriteColor());
    }

    private IEnumerator ChangeSpriteColor()
    {
        SpriteRenderer.material.shader = ShaderGUIText;
        yield return WhiteSpriteColorWait;
        SpriteRenderer.material.shader = ShaderDefault;
    }
}
