using System;
using System.Windows.Forms; // Necessário para MessageBox
using ScriptPortal.Vegas;

public class EntryPoint
{
    public void FromVegas(Vegas vegas)
    {
        try
        {
            // Percorre todas as faixas do projeto
            foreach (Track myTrack in vegas.Project.Tracks)
            {
                // Verifica se a faixa é do tipo de vídeo
                if (myTrack.IsVideo())
                {
                    // Percorre todos os eventos na faixa de vídeo
                    foreach (TrackEvent trackEvent in myTrack.Events)
                    {
                        // Verifica se o evento é do tipo VideoEvent e se está selecionado
                        VideoEvent vEvent = trackEvent as VideoEvent;
                        if (vEvent != null && vEvent.Selected)
                        {
                            // Acessa o fluxo de vídeo do evento
                            VideoStream vs = (VideoStream)vEvent.ActiveTake.Media.Streams.GetItemByMediaType(MediaType.Video, vEvent.ActiveTake.StreamIndex);

                            // Obtém a altura e largura do vídeo
                            int sHeight = vs.Height;
                            int sWidth = vs.Width;

                            // Verifica a rotação do vídeo
                            if (vs.Rotation == VideoOutputRotation.QuarterTurnClockwise || vs.Rotation == VideoOutputRotation.QuarterTurnCounterclockwise)
                            {
                                // Inverte a altura e largura em caso de rotação
                                sHeight = vs.Width;
                                sWidth = vs.Height;
                            }

                            // Limpa os keyframes de movimento (Pan/Crop)
                            vEvent.VideoMotion.Keyframes.Clear();

                            // Cria um novo keyframe de transformação (reseta os valores)
                            VideoMotionKeyframe kf = vEvent.VideoMotion.Keyframes[0];
                            VideoMotionBounds mb = kf.Bounds;

                            // Define os novos valores para o Pan/Crop
                            mb.TopLeft = new VideoMotionVertex(0f, 0f);
                            mb.TopRight = new VideoMotionVertex((float)sWidth, 0f);
                            mb.BottomLeft = new VideoMotionVertex(0f, (float)sHeight);
                            mb.BottomRight = new VideoMotionVertex((float)sWidth, (float)sHeight);

                            // Atribui os novos limites e reseta a rotação
                            kf.Bounds = mb;
                            kf.Rotation = 0.0f;
                        }
                    }
                }
            }

            // Exibe uma mensagem de sucesso usando MessageBox
            MessageBox.Show("Pan/Crop settings reset successfully!\nCreated By Gianluca Bracali Cargnelutti", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            // Caso ocorra algum erro, exibe a mensagem de erro
            MessageBox.Show("Erro: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
