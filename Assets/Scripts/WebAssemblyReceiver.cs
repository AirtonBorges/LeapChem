using UnityEngine;

public class WebAssemblyReceiver : MonoBehaviour
{
    public GameLogic gameLogic;

    public void AlterarTitulo(string titulo)
    {
        gameLogic.TextoTitulo.text = titulo;
    }

    public void AlterarDescricao(string descricao)
    {
        gameLogic.TextoDescricao.text = descricao;
    }

    public void AdicionarParticula(int tipo)
    {
        gameLogic.AdicionarParticula((EKindOfParticle)tipo);
    }

    public void LimparParticulas()
    {
        gameLogic.RemoverTodasAsParticulas();
    }
}
