$(function () {
    $(document).on("click", ".btn-adicionar-ementa", adicionarEmenta)
    $(document).on("click", ".btn-remover-ementa", removerEmenta)

    function adicionarEmenta() {
        let textEmenta = $(".descricao-ementa").val();

        $("#lista-ementa").append(`
                    <tr class="row item-ementa">
                        <td class="col-10">
                            <input type="hidden" class="dados-item-ementa" name="Ementas[0].Id" value="0" />
                            <input type="hidden" class="dados-item-ementa" name="Ementas[0].CursoId" value="0" />
                            <input type="hidden" class="dados-item-ementa" name="Ementas[0].Descricao" value="${textEmenta}" />

                            ${textEmenta}
                        </td>

                        <td class="col-2 text-end">
                            <button type="button" class="btn btn-sm btn-danger btn-remover-ementa" data-id-ementa="@ementa.Id"><i class="bi bi-trash"></i></button>
                        </td>
                    </tr>
                `)

        enumeracao();
        mostrarEmenta();
    }

    function removerEmenta() {
        var linha = $(this).closest("tr");

        $(linha).remove();

        enumeracao();
        mostrarEmenta();
    }

    function enumeracao() {
        var linhas = $(".item-ementa");

        $.each(linhas, function (i, linha) {
            var inputs = $(linha).find('.dados-item-ementa');

            $.each(inputs, function (j, input) {
                var name = $(input).attr("name");
                var prop = name.split('.')[1];
                name = "Ementas[" + i + "]." + prop;

                $(input).attr("name", name);
            });
        });
    }

    function mostrarEmenta() {
        var itensAreaAtuacao = $(".item-ementa");

        if (itensAreaAtuacao.length != 0) {
            $(".curso-sem-ementa").addClass("d-none");
            $(".curso-com-ementa").removeClass("d-none");
        } else {
            $(".curso-com-ementa").addClass("d-none");
            $(".curso-sem-ementa").removeClass("d-none");
        }
    }
});