$(function () {
    $(document).on("click", ".btn-adicionar-area-atuacao", adicionarAreaAtuacao)
    $(document).on("click", ".btn-remover-area-atuacao", removerAreasAtuacao)

    function adicionarAreaAtuacao() {
        let idAreaAtuacao = $(".areas-atuacoes").val();
        let textAreaAtuacao = $(".areas-atuacoes option:selected").text();

        $("#lista-areas-atuacao").append(`
                    <tr class="row item-area-atuacao">
                        <td class="col-10">
                            <input type="hidden" class="dados-item-area-atuacao" name="AreasAtuacao[0].Id" value="${idAreaAtuacao}" />
                            <input type="hidden" class="dados-item-area-atuacao" name="AreasAtuacao[0].Nome" value="${textAreaAtuacao}" />

                            ${textAreaAtuacao}
                        </td>

                        <td class="col-2 text-end">
                            <button type="button" class="btn btn-sm btn-danger btn-remover-area-atuacao" data-id-area-atuacao="${idAreaAtuacao}"><i class="bi bi-trash"></i></button>
                        </td>
                    </tr>
                `)

        enumeracao();
        mostrarAreasAtuacao();
    }

    function removerAreasAtuacao() {
        var linha = $(this).closest("tr");

        $(linha).remove();

        enumeracao();
        mostrarAreasAtuacao();
    }

    function enumeracao() {
        var linhas = $(".item-area-atuacao");

        $.each(linhas, function (i, linha) {
            var inputs = $(linha).find('.dados-item-area-atuacao');

            $.each(inputs, function (j, input) {
                var name = $(input).attr("name");
                var prop = name.split('.')[1];
                name = "AreasAtuacao[" + i + "]." + prop;

                $(input).attr("name", name);
            });
        });
    }

    function mostrarAreasAtuacao() {
        var itensAreaAtuacao = $(".item-area-atuacao");

        if (itensAreaAtuacao.length != 0) {
            $(".cooperado-sem-area-atuacao").addClass("d-none");
            $(".cooperado-com-area-atuacao").removeClass("d-none");
        } else {
            $(".cooperado-com-area-atuacao").addClass("d-none");
            $(".cooperado-sem-area-atuacao").removeClass("d-none");
        }
    }
});