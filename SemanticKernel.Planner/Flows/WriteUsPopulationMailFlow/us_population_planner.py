import asyncio
from promptflow import tool
from promptflow.connections import AzureOpenAIConnection
import semantic_kernel as sk
from semantic_kernel.planning.sequential_planner import SequentialPlanner
from plugins.UnitedStatesPlugin.UnitedStatesPlugin import UnitedStatesPlugin

from semantic_kernel.connectors.ai.open_ai import (
    AzureChatCompletion
)

# The inputs section will change based on the arguments of the tool function, after you save the code
# Adding type to arguments and return value will help the system show the types properly
# Please update the function name/signature per need
@tool
def my_python_tool(conn: AzureOpenAIConnection, deployment_name:str, input: str) -> str:

  kernel =sk.Kernel()

  kernel.add_chat_service("dv", AzureChatCompletion(deployment_name, conn.api_base, conn.api_key))

  skills_directory = "./plugins/"
  kernel.import_semantic_skill_from_directory(skills_directory, "MailPlugin")
  kernel.import_skill(UnitedStatesPlugin(), skill_name="UnitedStatesPlugin")

  planner = SequentialPlanner(kernel=kernel)

  ask = "Write a mail to share an update on the following information: " + input + ". If you don't have enough information, say that the requested data isn't currently available and you will follow up as soon as it's available."

  basic_plan = asyncio.run(planner.create_plan_async(ask))
  result = asyncio.run(kernel.run_async(basic_plan)).result

  return str(result)
