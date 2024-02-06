$(function () {
    $(document).on("click", ".btn-adicionar-pdi", adicionarAreaAtuacao)
    $(document).on("click", ".btn-remover-pdi", removerAreasAtuacao)

    function adicionarAreaAtuacao() {
        let idAreaAtuacao = $(".pdi").val();
        let textAreaAtuacao = $(".pdi option:selected").text();

        $("#lista-pdi").append(`
                    <tr class="row item-pdi">
                        <td class="col-10">
                            <input type="hidden" class="dados-item-pdi" name="PDIs[0].Id" value="${idAreaAtuacao}" />
                            <input type="hidden" class="dados-item-pdi" name="PDIs[0].Nome" value="${textAreaAtuacao}" />

                            ${textAreaAtuacao}
                        </td>

                        <td class="col-2 text-end">
                            <button type="button" class="btn btn-sm btn-danger btn-remover-pdi" data-id-pdi="${idAreaAtuacao}"><i class="bi bi-trash"></i></button>
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
        var linhas = $(".item-pdi");

        $.each(linhas, function (i, linha) {
            var inputs = $(linha).find('.dados-item-pdi');

            $.each(inputs, function (j, input) {
                var name = $(input).attr("name");
                var prop = name.split('.')[1];
                name = "PDIs[" + i + "]." + prop;

                $(input).attr("name", name);
            });
        });
    }

    function mostrarAreasAtuacao() {
        var itensAreaAtuacao = $(".item-pdi");

        if (itensAreaAtuacao.length != 0) {
            $(".cooperado-sem-pdi").addClass("d-none");
            $(".cooperado-com-pdi").removeClass("d-none");
        } else {
            $(".cooperado-com-pdi").addClass("d-none");
            $(".cooperado-sem-pdi").removeClass("d-none");
        }
    }
});