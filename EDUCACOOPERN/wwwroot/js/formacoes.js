$(function () {
    $(document).on("click", ".btn-adicionar-formacao", adicionarFormacao)
    $(document).on("click", ".btn-remover-formacao", removerFormacoes)

    function adicionarFormacao() {
        let tipo = $(".tipo-formacao option:selected").text();
        let tipoVal = $(".tipo-formacao option:selected").val();
        let textFormacao = $(".formacao").val();

        $("#lista-formacoes").append(`
                    <tr class="row item-formacao">
                        <td class="col-4">
                            <input type="hidden" class="dados-item-formacao" name="Formacoes[0].Nome" value="${textFormacao}" />
                            <input type="hidden" class="dados-item-formacao" name="Formacoes[0].Tipo" value="${tipoVal}" />

                            ${tipo}
                        </td>

                        <td class="col-6">
                            ${textFormacao}
                        </td>

                        <td class="col-2 text-end">
                            <button type="button" class="btn btn-sm btn-danger btn-remover-formacao"><i class="bi bi-trash"></i></button>
                        </td>
                    </tr>
                `)

        enumeracao();
        mostrarFormacoes();
    }

    function removerFormacoes() {
        var linha = $(this).closest("tr");

        $(linha).remove();

        enumeracao();
        mostrarFormacoes();
    }

    function enumeracao() {
        var linhas = $(".item-formacao");

        $.each(linhas, function (i, linha) {
            var inputs = $(linha).find('.dados-item-formacao');

            $.each(inputs, function (j, input) {
                var name = $(input).attr("name");
                var prop = name.split('.')[1];
                name = "Formacoes[" + i + "]." + prop;

                $(input).attr("name", name);
            });
        });
    }

    function mostrarFormacoes() {
        var itensAreaAtuacao = $(".item-formacao");

        if (itensAreaAtuacao.length != 0) {
            $(".cooperado-sem-formacao").addClass("d-none");
            $(".cooperado-com-formacao").removeClass("d-none");
        } else {
            $(".cooperado-com-formacao").addClass("d-none");
            $(".cooperado-sem-formacao").removeClass("d-none");
        }
    }
});