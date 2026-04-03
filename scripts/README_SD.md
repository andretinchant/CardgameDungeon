# Stable Diffusion Local para Arte das Cartas

Este fluxo troca a geração via DALL-E por Stable Diffusion rodando localmente no AUTOMATIC1111 WebUI, usando a API em `http://127.0.0.1:7860/sdapi/v1/txt2img`.

## 1. Pré-requisitos no Windows

1. Instale o Python `3.10.6` 64-bit e marque `Add Python to PATH`.
2. Instale o Git for Windows.
3. Garanta que você tenha uma GPU compatível com CUDA para o fluxo mais simples do AUTOMATIC1111.

Referência oficial do AUTOMATIC1111:

- Repositório: https://github.com/AUTOMATIC1111/stable-diffusion-webui
- Guia de instalação para Windows/NVIDIA: https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki/Install-and-Run-on-NVidia-GPUs

## 2. Instalar o AUTOMATIC1111 WebUI

Abra `cmd` ou PowerShell em uma pasta fora deste repositório e rode:

```powershell
git clone https://github.com/AUTOMATIC1111/stable-diffusion-webui.git
cd stable-diffusion-webui
```

Na primeira execução, o WebUI cria o ambiente e baixa dependências automaticamente:

```powershell
webui-user.bat
```

Feche depois que confirmar que a instalação abriu corretamente no navegador.

## 3. Baixar um modelo recomendado para fantasy art

Modelos recomendados para este projeto:

### Opção recomendada: DreamShaper 8

- Indicado quando você quer fantasia ilustrativa com boa mistura de painterly + semi-realista.
- Página do modelo: https://huggingface.co/Lykon/dreamshaper-8

### Alternativa: Realistic Vision V6.0 B1

- Indicado se você quiser puxar mais para realismo e retrato.
- Página do modelo: https://huggingface.co/SG161222/Realistic_Vision_V6.0_B1_noVAE

Depois de baixar o checkpoint (`.safetensors` ou `.ckpt`), copie o arquivo para:

```text
stable-diffusion-webui\models\Stable-diffusion\
```

Se usar `Realistic Vision V6.0 B1 noVAE`, o model card recomenda também o VAE:

- https://huggingface.co/stabilityai/sd-vae-ft-mse-original

Nesse caso, coloque o VAE conforme sua convenção local do AUTOMATIC1111 e selecione-o na interface, se necessário.

## 4. Iniciar a API local

Dentro da pasta `stable-diffusion-webui`, suba o WebUI com API habilitada:

```powershell
webui.bat --api
```

Quando estiver pronto:

- UI: `http://127.0.0.1:7860`
- Swagger/OpenAPI: `http://127.0.0.1:7860/docs`

## 5. Script deste repositório

O script novo está em:

```text
scripts\generate_sd_card_art.py
```

Ele:

- lê o style guide em `STYLE_GUIDE_DND_DUNGEON.md`
- extrai o prompt base e o negative prompt
- varre os seeds C# em `src/CardgameDungeon.API/Data/Seeds`
- monta prompts por carta
- chama `POST /sdapi/v1/txt2img`
- salva PNGs em `art-output/stable-diffusion`

Parâmetros fixos da geração:

```json
{
  "width": 512,
  "height": 896,
  "steps": 30,
  "cfg_scale": 7.5,
  "sampler_name": "DPM++ 2M Karras",
  "batch_size": 1
}
```

## 6. Executar em modo de parsing

Para só validar parsing de cartas e prompts, sem chamar a API:

```powershell
python scripts/generate_sd_card_art.py --parse-only
```

Esse modo gera um manifesto em:

```text
art-output\stable-diffusion\manifest.json
```

## 7. Executar geração real

Exemplo gerando tudo que o script encontrar:

```powershell
python scripts/generate_sd_card_art.py
```

Exemplos úteis com filtros:

```powershell
python scripts/generate_sd_card_art.py --set-code DND1 --card-type Monster --limit 10
python scripts/generate_sd_card_art.py --name "Ancient Red Dragon" --overwrite
```

## 8. Observações práticas

- O script assume a API local em `http://127.0.0.1:7860`, mas aceita `--base-url` se você mudar a porta.
- Se a imagem já existir, ele pula por padrão; use `--overwrite` para regerar.
- Se o WebUI não estiver no ar, o script falha com erro de conexão.
- Para conferir nomes de checkpoints e opções expostas pela sua instância, use `http://127.0.0.1:7860/docs`.
