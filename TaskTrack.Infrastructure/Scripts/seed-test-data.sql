-- Massa de dados para ambiente de testes (PostgreSQL)
-- Modelo atual: solicitacoes com coluna localizacao (sem local_pai/local_filho)

BEGIN;

-- ------------------------------------------------------------
-- 1) Limpeza idempotente (apenas IDs desta massa)
-- ------------------------------------------------------------
DELETE FROM planejamento_materiais
WHERE id IN (
    'cccccccc-0000-0000-0000-000000000001',
    'cccccccc-0000-0000-0000-000000000002',
    'cccccccc-0000-0000-0000-000000000003',
    'cccccccc-0000-0000-0000-000000000004',
    'cccccccc-0000-0000-0000-000000000005'
);

DELETE FROM planejamento_responsaveis
WHERE id IN (
    'dddddddd-0000-0000-0000-000000000001',
    'dddddddd-0000-0000-0000-000000000002',
    'dddddddd-0000-0000-0000-000000000003',
    'dddddddd-0000-0000-0000-000000000004'
);

DELETE FROM aprovacoes_solicitacao
WHERE id IN (
    'eeeeeeee-0000-0000-0000-000000000001',
    'eeeeeeee-0000-0000-0000-000000000002',
    'eeeeeeee-0000-0000-0000-000000000003'
);

DELETE FROM execucoes
WHERE id IN (
    'ffffffff-0000-0000-0000-000000000001',
    'ffffffff-0000-0000-0000-000000000002',
    'ffffffff-0000-0000-0000-000000000003',
    'ffffffff-0000-0000-0000-000000000004',
    'ffffffff-0000-0000-0000-000000000005'
);

DELETE FROM planejamentos
WHERE id IN (
    'bbbbbbbb-0000-0000-0000-000000000001',
    'bbbbbbbb-0000-0000-0000-000000000002',
    'bbbbbbbb-0000-0000-0000-000000000003'
);

DELETE FROM solicitacoes
WHERE id IN (
    'aaaaaaaa-0000-0000-0000-000000000001',
    'aaaaaaaa-0000-0000-0000-000000000002',
    'aaaaaaaa-0000-0000-0000-000000000003',
    'aaaaaaaa-0000-0000-0000-000000000004',
    'aaaaaaaa-0000-0000-0000-000000000005'
);

DELETE FROM "AspNetUsers"
WHERE "Id" IN (
    '11111111-1111-1111-1111-111111111111',
    '22222222-2222-2222-2222-222222222222',
    '33333333-3333-3333-3333-333333333333'
);

-- ------------------------------------------------------------
-- 2) Usuarios base
-- ------------------------------------------------------------
INSERT INTO "AspNetUsers" (
    "Id",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash",
    "SecurityStamp",
    "ConcurrencyStamp",
    "PhoneNumber",
    "PhoneNumberConfirmed",
    "TwoFactorEnabled",
    "LockoutEnd",
    "LockoutEnabled",
    "AccessFailedCount"
)
VALUES
(
    '11111111-1111-1111-1111-111111111111',
    'solicitante.01',
    'SOLICITANTE.01',
    'solicitante.01@tasktrack.local',
    'SOLICITANTE.01@TASKTRACK.LOCAL',
    true,
    null,
    'sec-1111',
    'con-1111',
    null,
    false,
    false,
    null,
    true,
    0
),
(
    '22222222-2222-2222-2222-222222222222',
    'solicitante.02',
    'SOLICITANTE.02',
    'solicitante.02@tasktrack.local',
    'SOLICITANTE.02@TASKTRACK.LOCAL',
    true,
    null,
    'sec-2222',
    'con-2222',
    null,
    false,
    false,
    null,
    true,
    0
),
(
    '33333333-3333-3333-3333-333333333333',
    'gestor.01',
    'GESTOR.01',
    'gestor.01@tasktrack.local',
    'GESTOR.01@TASKTRACK.LOCAL',
    true,
    null,
    'sec-3333',
    'con-3333',
    null,
    false,
    false,
    null,
    true,
    0
);

-- ------------------------------------------------------------
-- 3) Solicitacoes
-- ------------------------------------------------------------
INSERT INTO solicitacoes (
    id,
    titulo,
    descricao,
    localizacao,
    status,
    data_criacao,
    solicitante_id
)
VALUES
(
    'aaaaaaaa-0000-0000-0000-000000000001',
    'Troca de luminarias corredor principal',
    'Luminarias piscando no turno da noite.',
    'Predio 2, bloco B, 1, 2, 3 e 4 andares',
    0,
    NOW() - INTERVAL '12 days',
    '11111111-1111-1111-1111-111111111111'
),
(
    'aaaaaaaa-0000-0000-0000-000000000002',
    'Ajuste de rede no laboratorio de testes',
    'Perda intermitente de conectividade em bancada.',
    'Predio 1, bloco A, laboratorio 05',
    1,
    NOW() - INTERVAL '9 days',
    '11111111-1111-1111-1111-111111111111'
),
(
    'aaaaaaaa-0000-0000-0000-000000000003',
    'Reparo de infiltracao sala de reuniao',
    'Necessario avaliar forro e pintura.',
    'Predio administrativo, 2 andar, sala 12',
    2,
    NOW() - INTERVAL '6 days',
    '22222222-2222-2222-2222-222222222222'
),
(
    'aaaaaaaa-0000-0000-0000-000000000004',
    'Instalacao de ponto adicional de energia',
    'Novo ponto para bancada de impressoras.',
    'Centro logistico, galpao 3, setor expedicao',
    1,
    NOW() - INTERVAL '4 days',
    '22222222-2222-2222-2222-222222222222'
),
(
    'aaaaaaaa-0000-0000-0000-000000000005',
    'Manutencao preventiva de ar condicionado',
    'Unidade sem resfriamento adequado.',
    'Predio 3, bloco C, 5 andar, sala 501',
    0,
    NOW() - INTERVAL '2 days',
    '11111111-1111-1111-1111-111111111111'
);

-- ------------------------------------------------------------
-- 4) Aprovacoes
-- ------------------------------------------------------------
INSERT INTO aprovacoes_solicitacao (
    id,
    solicitacao_id,
    gestor_id,
    aprovado,
    observacao,
    data_aprovacao
)
VALUES
(
    'eeeeeeee-0000-0000-0000-000000000001',
    'aaaaaaaa-0000-0000-0000-000000000001',
    '33333333-3333-3333-3333-333333333333',
    true,
    'Aprovado para execucao no proximo ciclo.',
    NOW() - INTERVAL '11 days'
),
(
    'eeeeeeee-0000-0000-0000-000000000002',
    'aaaaaaaa-0000-0000-0000-000000000002',
    '33333333-3333-3333-3333-333333333333',
    true,
    'Prioridade media confirmada.',
    NOW() - INTERVAL '8 days'
),
(
    'eeeeeeee-0000-0000-0000-000000000003',
    'aaaaaaaa-0000-0000-0000-000000000003',
    '33333333-3333-3333-3333-333333333333',
    false,
    'Necessario complementar escopo e custos.',
    NOW() - INTERVAL '5 days'
);

-- ------------------------------------------------------------
-- 5) Planejamentos
-- ------------------------------------------------------------
INSERT INTO planejamentos (
    id,
    solicitacao_id,
    data_inicio_prevista,
    data_fim_prevista,
    observacoes
)
VALUES
(
    'bbbbbbbb-0000-0000-0000-000000000001',
    'aaaaaaaa-0000-0000-0000-000000000001',
    NOW() - INTERVAL '10 days',
    NOW() - INTERVAL '7 days',
    'Troca em duas etapas para nao impactar fluxo.'
),
(
    'bbbbbbbb-0000-0000-0000-000000000002',
    'aaaaaaaa-0000-0000-0000-000000000002',
    NOW() - INTERVAL '8 days',
    NOW() - INTERVAL '6 days',
    'Agendar janela fora do horario comercial.'
),
(
    'bbbbbbbb-0000-0000-0000-000000000003',
    'aaaaaaaa-0000-0000-0000-000000000004',
    NOW() - INTERVAL '3 days',
    NOW() + INTERVAL '2 days',
    'Dependente da chegada de materiais eletricos.'
);

-- ------------------------------------------------------------
-- 6) Materiais de planejamento
-- ------------------------------------------------------------
INSERT INTO planejamento_materiais (
    id,
    planejamento_id,
    nome,
    quantidade
)
VALUES
(
    'cccccccc-0000-0000-0000-000000000001',
    'bbbbbbbb-0000-0000-0000-000000000001',
    'Lampada LED 18W',
    24.00
),
(
    'cccccccc-0000-0000-0000-000000000002',
    'bbbbbbbb-0000-0000-0000-000000000001',
    'Reator eletronico',
    12.00
),
(
    'cccccccc-0000-0000-0000-000000000003',
    'bbbbbbbb-0000-0000-0000-000000000002',
    'Patch cord CAT6 2m',
    30.00
),
(
    'cccccccc-0000-0000-0000-000000000004',
    'bbbbbbbb-0000-0000-0000-000000000002',
    'Conector RJ45',
    60.00
),
(
    'cccccccc-0000-0000-0000-000000000005',
    'bbbbbbbb-0000-0000-0000-000000000003',
    'Tomada 20A',
    8.00
);

-- ------------------------------------------------------------
-- 7) Responsaveis do planejamento
-- ------------------------------------------------------------
INSERT INTO planejamento_responsaveis (
    id,
    planejamento_id,
    usuario_id
)
VALUES
(
    'dddddddd-0000-0000-0000-000000000001',
    'bbbbbbbb-0000-0000-0000-000000000001',
    '33333333-3333-3333-3333-333333333333'
),
(
    'dddddddd-0000-0000-0000-000000000002',
    'bbbbbbbb-0000-0000-0000-000000000002',
    '33333333-3333-3333-3333-333333333333'
),
(
    'dddddddd-0000-0000-0000-000000000003',
    'bbbbbbbb-0000-0000-0000-000000000003',
    '11111111-1111-1111-1111-111111111111'
),
(
    'dddddddd-0000-0000-0000-000000000004',
    'bbbbbbbb-0000-0000-0000-000000000003',
    '22222222-2222-2222-2222-222222222222'
);

-- ------------------------------------------------------------
-- 8) Execucoes
-- ------------------------------------------------------------
INSERT INTO execucoes (
    id,
    solicitacao_id,
    status,
    data_inicio_real,
    data_fim_real,
    atualizado_em,
    atualizado_por_id,
    observacao_atualizacao
)
VALUES
(
    'ffffffff-0000-0000-0000-000000000001',
    'aaaaaaaa-0000-0000-0000-000000000001',
    2,
    NOW() - INTERVAL '10 days',
    NOW() - INTERVAL '8 days',
    NOW() - INTERVAL '8 days',
    '33333333-3333-3333-3333-333333333333',
    'Servico concluido sem bloqueios.'
),
(
    'ffffffff-0000-0000-0000-000000000002',
    'aaaaaaaa-0000-0000-0000-000000000002',
    1,
    NOW() - INTERVAL '7 days',
    null,
    NOW() - INTERVAL '1 days',
    '33333333-3333-3333-3333-333333333333',
    'Execucao em andamento com equipe de redes.'
),
(
    'ffffffff-0000-0000-0000-000000000003',
    'aaaaaaaa-0000-0000-0000-000000000003',
    0,
    null,
    null,
    NOW() - INTERVAL '5 days',
    null,
    'Aguardando revisao de aprovacao.'
),
(
    'ffffffff-0000-0000-0000-000000000004',
    'aaaaaaaa-0000-0000-0000-000000000004',
    1,
    NOW() - INTERVAL '2 days',
    null,
    NOW() - INTERVAL '3 hours',
    '11111111-1111-1111-1111-111111111111',
    'Instalacao iniciada, faltam acabamentos.'
),
(
    'ffffffff-0000-0000-0000-000000000005',
    'aaaaaaaa-0000-0000-0000-000000000005',
    0,
    null,
    null,
    NOW() - INTERVAL '2 days',
    null,
    'Solicitacao registrada e aguardando triagem.'
);

COMMIT;
