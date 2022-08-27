using System.Collections;
using UnityEngine;

public class MobHealth : AbstractHealth
{
    [SerializeField] private MobHolder MobHolder;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private MobHolderGameEvent OnMobZeroHealth;
    private Shader ShaderGUIText;
    private Shader ShaderDefault;
    private Coroutine SpriteColorCor;
    private const float WhiteSpriteColorTime = 0.3f;
    private WaitForSeconds WhiteSpriteColorWait;

    protected override void Awake()
    {
        base.Awake();
        WhiteSpriteColorWait = new(WhiteSpriteColorTime);
        ShaderDefault = SpriteRenderer.material.shader;
        ShaderGUIText = Shader.Find("GUI/Text Shader");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SpriteRenderer.material.shader = ShaderDefault;
    }

    protected override void CommonDamage(float damage)
    {
        base.CommonDamage(damage);

        SpriteColorCor = StartCoroutine(ChangeSpriteColor());

        if (CurrentHealth == 0)
        {
            OnMobZeroHealth.Raise(MobHolder);
        }
    }

    private IEnumerator ChangeSpriteColor()
    {
        SpriteRenderer.material.shader = ShaderGUIText;
        yield return WhiteSpriteColorWait;
        SpriteRenderer.material.shader = ShaderDefault;
    }
}
