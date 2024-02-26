$(function () {
    $(document).on("click", ".btn-adicionar-custo", adicionarCusto)
    $(document).on("click", ".btn-remover-custo", removerCusto)

    function adicionarCusto() {
        let valor = $(".custo-valor").val();
        let classificacao = $(".custo-classificacao").val();
        let textClassificacao = $(".custo-classificacao option:selected").text();

        $("#lista-custos").append(`
                    <tr class="row item-custo">
                        <td class="col-8">
                            <input type="hidden" class="dados-item-custo" name="Custos[@index].Valor" value="${valor}" />
                            <input type="hidden" class="dados-item-custo" name="Custos[@index].Classificacao" value="${classificacao}" />

                            ${textClassificacao}
                        </td>
                        <td class="col-2">
                            R$ ${valor}
                        </td>

                        <td class="col-2 text-end">
                            <button type="button" class="btn btn-sm btn-danger btn-remover-custo"><i class="bi bi-trash"></i></button>
                        </td>
                    </tr>
                `)

        enumeracao();
        mostrarCustos();
    }

    function removerCusto() {
        var linha = $(this).closest("tr");

        $(linha).remove();

        enumeracao();
        mostrarCustos();
    }

    function enumeracao() {
        var linhas = $(".item-custo");

        $.each(linhas, function (i, linha) {
            var inputs = $(linha).find('.dados-item-custo');

            $.each(inputs, function (j, input) {
                var name = $(input).attr("name");
                var prop = name.split('.')[1];
                name = "Custos[" + i + "]." + prop;

                $(input).attr("name", name);
            });
        });
    }

    function mostrarCustos() {
        var itensCusto = $(".dados-item-custo");

        if (itensCusto.length != 0) {
            $(".cooperado-sem-custos").addClass("d-none");
            $(".cooperado-com-custos").removeClass("d-none");
        } else {
            $(".cooperado-com-custos").addClass("d-none");
            $(".cooperado-sem-custos").removeClass("d-none");
        }
    }
});