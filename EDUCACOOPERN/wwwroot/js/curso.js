$(function () {
    $(document).on("click", ".btn-adicionar-curso", adicionarAreaAtuacao)
    $(document).on("click", ".btn-remover-curso", removerAreasAtuacao)

    function adicionarAreaAtuacao() {
        let idAreaAtuacao = $(".curso").val();
        let textAreaAtuacao = $(".curso option:selected").text();

        $("#lista-curso").append(`
                    <tr class="row item-curso">
                        <td class="col-10">
                            <input type="hidden" class="dados-item-curso" name="Cursos[0].Id" value="${idAreaAtuacao}" />
                            <input type="hidden" class="dados-item-curso" name="Cursos[0].Nome" value="${textAreaAtuacao}" />

                            ${textAreaAtuacao}
                        </td>

                        <td class="col-2 text-end">
                            <button type="button" class="btn btn-sm btn-danger btn-remover-curso" data-id-curso="${idAreaAtuacao}"><i class="bi bi-trash"></i></button>
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
        var linhas = $(".item-curso");

        $.each(linhas, function (i, linha) {
            var inputs = $(linha).find('.dados-item-curso');

            $.each(inputs, function (j, input) {
                var name = $(input).attr("name");
                var prop = name.split('.')[1];
                name = "Cursos[" + i + "]." + prop;

                $(input).attr("name", name);
            });
        });
    }

    function mostrarAreasAtuacao() {
        var itensAreaAtuacao = $(".item-curso");

        if (itensAreaAtuacao.length != 0) {
            $(".cooperado-sem-curso").addClass("d-none");
            $(".cooperado-com-curso").removeClass("d-none");
        } else {
            $(".cooperado-com-curso").addClass("d-none");
            $(".cooperado-sem-curso").removeClass("d-none");
        }
    }
});